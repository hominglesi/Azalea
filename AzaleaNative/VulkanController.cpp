#include <iostream>
#include <algorithm>
#include <vector>
#include <array>
#include <optional>
#include <set>
#include "DataTypes.h"

#define VK_USE_PLATFORM_WIN32_KHR
#include "vulkan/vulkan.h"

#define THROW(error) \
	std::cout << "ERROR: " error << '\n'; \
	throw std::runtime_error(error);

const std::vector<const char*> validationLayers = {
	"VK_LAYER_KHRONOS_validation"
};

const std::vector<const char*> instanceExtentions = {
	"VK_KHR_surface",
	"VK_KHR_win32_surface"
};

const std::vector<const char*> deviceExtensions = {
	"VK_KHR_swapchain"
};

const std::vector<VkDynamicState> dynamicStates = {
	VK_DYNAMIC_STATE_VIEWPORT,
	VK_DYNAMIC_STATE_SCISSOR
};

const int MAX_VERTEX_INDEX = 65535;
const int VERTEX_BUFFER_LENGTH = 65536;
const int INDEX_BUFFER_LENGTH = 98304;

class VulkanController {

public:

	VulkanController(HWND hwnd, HINSTANCE hInstance, byte* vertShaderData, int vertShaderLength, byte* fragShaderData, int fragShaderLength) {
		window = hwnd;
		vulkanInstance = createVulkanInstance();
		targetSurface = createWindowsSurface(vulkanInstance, window, hInstance);
		physicalDevice = selectPhysicalDevice(vulkanInstance, targetSurface);
		queueFamilies = getQueueFamilies(physicalDevice, targetSurface);
		swapchainSupport = getSwapchainSupport(physicalDevice, targetSurface);
		device = createDevice(physicalDevice, queueFamilies);
		graphicsQueue = getGraphicsQueue(device, queueFamilies);
		presentQueue = getPresentQueue(device, queueFamilies);
		swapchain = createSwapchain(device, targetSurface, swapchainSupport, queueFamilies, window);
		swapchainImageFormat = chooseSwapSurfaceFormat(swapchainSupport.formats).format;
		swapchainExtent = chooseSwapExtent(swapchainSupport.capabilities, window);
		swapchainImages = getSwapchainImages(device, swapchain);
		swapchainImageViews = createSwapchainImageViews(device, swapchainImages, swapchainImageFormat);
		descriptorPool = createDescriptorPool(device);
		descriptorSetLayout = createDescriptorSetLayout(device);
		descriptorSet = createDescriptorSet(device, descriptorPool, descriptorSetLayout);
		CreateUniformBuffer();
		pipelineLayout = createPipelineLayout(device, descriptorSetLayout);
		renderPass = createRenderPass(device, swapchainImageFormat);
		graphicsPipeline = createGraphicsPipeline(device, pipelineLayout, renderPass, vertShaderData, vertShaderLength, fragShaderData, fragShaderLength, swapchainExtent);
		framebuffers = createFramebuffers(device, swapchainImageViews, renderPass, swapchainExtent);
		commandPool = createCommandPool(device, queueFamilies);
		commandBuffer = createCommandBuffer(device, commandPool);
		textureSampler = createTextureSampler(physicalDevice, device);

		imageAvalibleSemaphore = createSemaphore(device);
		renderFinishedSemaphore = createSemaphore(device);
		isRenderingFence = createFence(device, true);

		CreateIndexBuffer();
		CreateVertexBuffer();
		uniformBufferObject.projection = matrix4x4::identity();
	}

	~VulkanController() {
		vkDeviceWaitIdle(device);

		cleanupSwapchain(device, framebuffers, swapchainImageViews, swapchain);

		destroySampler(device, textureSampler);
		destroyImageView(device, textureImageView);
		destroyImage(device, textureImage);
		freeDeviceMemory(device, textureImageMemory);
		destroyBuffer(device, indexBuffer);
		freeDeviceMemory(device, indexBufferMemory);
		destroyBuffer(device, vertexBuffer);
		vkUnmapMemory(device, vertexBufferMemory);
		freeDeviceMemory(device, vertexBufferMemory);
		destroyBuffer(device, uniformBuffer);
		vkUnmapMemory(device, uniformBufferMemory);
		freeDeviceMemory(device, uniformBufferMemory);
		destroyFence(device, isRenderingFence);
		destroySemaphore(device, renderFinishedSemaphore);
		destroySemaphore(device, imageAvalibleSemaphore);
		destroyCommandPool(device, commandPool);
		destroyGraphicsPipeline(device, graphicsPipeline);
		destroyRenderPass(device, renderPass);
		destroyDescriptorPool(device, descriptorPool);
		destroyDescriptorSetLayout(device, descriptorSetLayout);
		destroyPipelineLayout(device, pipelineLayout);
		destroyDevice(device);
		destroySurface(vulkanInstance, targetSurface);
		destroyVulkanInstance(vulkanInstance);
		std::cout << "VulkanController successfully destroyed!\n";
	}

	void CreateIndexBuffer() {
		uint16_t* indexBufferData = new uint16_t[INDEX_BUFFER_LENGTH];
		for (int i = 0, j = 0; i < INDEX_BUFFER_LENGTH; i += 6, j += 4)
		{
			indexBufferData[i] = j;
			indexBufferData[i + 1] = j + 1;
			indexBufferData[i + 2] = j + 2;
			indexBufferData[i + 3] = j + 2;
			indexBufferData[i + 4] = j + 3;
			indexBufferData[i + 5] = j;
		}

		VkDeviceSize indexBufferSize = sizeof(indexBufferData[0]) * INDEX_BUFFER_LENGTH;

		VkBuffer indexStagingBuffer;
		VkDeviceMemory indexStagingBufferMemory;

		indexStagingBuffer = createBuffer(device, indexBufferSize, VK_BUFFER_USAGE_TRANSFER_SRC_BIT);
		indexStagingBufferMemory = allocateBufferMemory(physicalDevice, device, indexStagingBuffer, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

		void* data;
		vkMapMemory(device, indexStagingBufferMemory, 0, indexBufferSize, 0, &data);
		memcpy(data, indexBufferData, (size_t)indexBufferSize);
		vkUnmapMemory(device, indexStagingBufferMemory);

		indexBuffer = createBuffer(device, indexBufferSize, VK_BUFFER_USAGE_TRANSFER_DST_BIT | VK_BUFFER_USAGE_INDEX_BUFFER_BIT);
		indexBufferMemory = allocateBufferMemory(physicalDevice, device, indexBuffer, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

		copyBuffer(device, commandPool, graphicsQueue, indexStagingBuffer, indexBuffer, indexBufferSize);

		destroyBuffer(device, indexStagingBuffer);
		freeDeviceMemory(device, indexStagingBufferMemory);
		delete[] indexBufferData;
	}

	void CreateVertexBuffer() {
		VkDeviceSize vertexBufferSize = sizeof(Vertex) * VERTEX_BUFFER_LENGTH;

		vertexBuffer = createBuffer(device, vertexBufferSize, VK_BUFFER_USAGE_VERTEX_BUFFER_BIT);
		vertexBufferMemory = allocateBufferMemory(physicalDevice, device, vertexBuffer, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

		if (vkMapMemory(device, vertexBufferMemory, 0, vertexBufferSize, 0, &vertexBufferMapping) != VK_SUCCESS) {
			THROW("Could not map vertex buffer memory!");
		}
	}

	void CreateUniformBuffer() {
		VkDeviceSize uniformBufferSize = sizeof(UniformBufferObject);

		uniformBuffer = createBuffer(device, uniformBufferSize, VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT);
		uniformBufferMemory = allocateBufferMemory(physicalDevice, device, uniformBuffer, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

		if (vkMapMemory(device, uniformBufferMemory, 0, uniformBufferSize, 0, &uniformBufferMapping) != VK_SUCCESS) {
			THROW("Could not map uniform buffer memory!");
		};
	}

	void CreateTexture(int width, int height, int channelCount, void* textureData) {
		VkDeviceSize imageSize = width * height * 4;

		VkBuffer stagingBuffer = createBuffer(device, imageSize, VK_BUFFER_USAGE_TRANSFER_SRC_BIT);
		VkDeviceMemory stagingBufferMemory = allocateBufferMemory(physicalDevice, device, stagingBuffer, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

		void* data;
		vkMapMemory(device, stagingBufferMemory, 0, imageSize, 0, &data);
		memcpy(data, textureData, static_cast<size_t>(imageSize));
		vkUnmapMemory(device, stagingBufferMemory);

		textureImage = createTextureImage(device, width, height, channelCount, textureData);
		textureImageMemory = allocateImageMemory(physicalDevice, device, textureImage);

		transitionImageLayout(device, commandPool, graphicsQueue, textureImage, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_LAYOUT_UNDEFINED, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL);
		copyBufferToImage(device, commandPool, graphicsQueue, stagingBuffer, textureImage, width, height);

		transitionImageLayout(device, commandPool, graphicsQueue, textureImage, VK_FORMAT_R8G8B8A8_SRGB, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL);
	
		destroyBuffer(device, stagingBuffer);
		freeDeviceMemory(device, stagingBufferMemory);

		textureImageView = createImageView(device, textureImage, VK_FORMAT_R8G8B8A8_SRGB);
		configureUniformDescriptor(device, uniformBuffer, textureImageView, textureSampler, descriptorSet);
	}

	void BeginFrame() {
		vkWaitForFences(device, 1, &isRenderingFence, VK_TRUE, UINT64_MAX);

		VkResult result;
		result = vkAcquireNextImageKHR(device, swapchain, UINT32_MAX, imageAvalibleSemaphore, VK_NULL_HANDLE, &currentImageIndex);

		if (result == VK_ERROR_OUT_OF_DATE_KHR) {
			RecreateSwapchain();
			return;
		}
		else if (result != VK_SUCCESS && result != VK_SUBOPTIMAL_KHR) {
			THROW("Could not acquire swapchain image!");
		}

		vkResetFences(device, 1, &isRenderingFence);
		vkResetCommandBuffer(commandBuffer, 0);
		currentVertexCount = 0;
		currentIndexCount = 0;
		vertexBufferData.clear();

		VkCommandBufferBeginInfo beginInfo{};
		beginInfo.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO;

		if (vkBeginCommandBuffer(commandBuffer, &beginInfo) != VK_SUCCESS) {
			THROW("Could not begin recording command buffer!");
		}

		VkRenderPassBeginInfo renderPassInfo{};
		renderPassInfo.sType = VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO;
		renderPassInfo.renderPass = renderPass;
		renderPassInfo.framebuffer = framebuffers[currentImageIndex];
		renderPassInfo.renderArea.offset = { 0, 0 };
		renderPassInfo.renderArea.extent = swapchainExtent;

		VkClearValue clearColor = { {{ 0.0f, 0.0f, 0.0f, 1.0f }} };
		renderPassInfo.clearValueCount = 1;
		renderPassInfo.pClearValues = &clearColor;

		vkCmdBeginRenderPass(commandBuffer, &renderPassInfo, VK_SUBPASS_CONTENTS_INLINE);
		vkCmdBindPipeline(commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, graphicsPipeline);

		VkBuffer vertexBuffers[] = { vertexBuffer };
		VkDeviceSize offsets[] = { 0 };
		vkCmdBindVertexBuffers(commandBuffer, 0, 1, vertexBuffers, offsets);

		vkCmdBindIndexBuffer(commandBuffer, indexBuffer, 0, VK_INDEX_TYPE_UINT16);

		VkViewport viewport{};
		viewport.x = 0.0f;
		viewport.y = 0.0f;
		viewport.width = static_cast<float>(swapchainExtent.width);
		viewport.height = static_cast<float>(swapchainExtent.height);
		viewport.minDepth = 0.0f;
		viewport.maxDepth = 1.0f;
		vkCmdSetViewport(commandBuffer, 0, 1, &viewport);

		VkRect2D scissor{};
		scissor.offset = { 0, 0 };
		scissor.extent = swapchainExtent;
		vkCmdSetScissor(commandBuffer, 0, 1, &scissor);

		vkCmdBindDescriptorSets(commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayout, 0, 1, &descriptorSet, 0, nullptr);
	}

	void PushQuad(vector2 topLeft, vector2 topRight, vector2 bottomRight, vector2 bottomLeft) {
		vertexBufferData.push_back({ topLeft,     { 1.0f, 1.0f, 1.0f }, 0.0f, { 0.0f, 0.0f } });
		vertexBufferData.push_back({ topRight,    { 1.0f, 1.0f, 1.0f }, 0.0f, { 1.0f, 0.0f } });
		vertexBufferData.push_back({ bottomRight, { 1.0f, 1.0f, 1.0f }, 0.0f, { 1.0f, 1.0f } });
		vertexBufferData.push_back({ bottomLeft,  { 1.0f, 1.0f, 1.0f }, 0.0f, { 0.0f, 1.0f } });

		currentVertexCount += 4;
		currentIndexCount += 6;
	}

	void FinishFrame() {
		vkCmdDrawIndexed(commandBuffer, currentIndexCount, 1, 0, 0, 0);
		vkCmdEndRenderPass(commandBuffer);

		if (vkEndCommandBuffer(commandBuffer) != VK_SUCCESS) {
			THROW("Failed to record framebuffer!");
		}

		updateUniformBuffer(swapchainExtent, uniformBufferMapping, uniformBufferObject);
		updateVertexBuffer(vertexBufferMapping, vertexBufferData.data(), sizeof(Vertex) * currentVertexCount);

		VkSubmitInfo submitInfo{};
		submitInfo.sType = VK_STRUCTURE_TYPE_SUBMIT_INFO;

		VkSemaphore waitSemaphores[] = { imageAvalibleSemaphore };
		VkPipelineStageFlags waitStages[] = { VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT };
		submitInfo.waitSemaphoreCount = 1;
		submitInfo.pWaitSemaphores = waitSemaphores;
		submitInfo.pWaitDstStageMask = waitStages;

		submitInfo.commandBufferCount = 1;
		submitInfo.pCommandBuffers = &commandBuffer;

		VkSemaphore signalSemaphores[] = { renderFinishedSemaphore };
		submitInfo.signalSemaphoreCount = 1;
		submitInfo.pSignalSemaphores = signalSemaphores;

		if (vkQueueSubmit(graphicsQueue, 1, &submitInfo, isRenderingFence) != VK_SUCCESS) {
			THROW("Could not submit draw command buffer");
		}

		VkResult result = presentSwapchain(presentQueue, swapchain, currentImageIndex, renderFinishedSemaphore);

		if (result == VK_ERROR_OUT_OF_DATE_KHR || result == VK_SUBOPTIMAL_KHR || framebufferResized) {
			framebufferResized = false;
			RecreateSwapchain();
		}
		else if (result != VK_SUCCESS) {
			THROW("Could not present swapchain image!");
		}
	}

	void RecreateSwapchain() {
		vkDeviceWaitIdle(device);

		cleanupSwapchain(device, framebuffers, swapchainImageViews, swapchain);

		swapchainSupport = getSwapchainSupport(physicalDevice, targetSurface);
		swapchain = createSwapchain(device, targetSurface, swapchainSupport, queueFamilies, window);
		swapchainImageFormat = chooseSwapSurfaceFormat(swapchainSupport.formats).format;
		swapchainExtent = chooseSwapExtent(swapchainSupport.capabilities, window);
		swapchainImages = getSwapchainImages(device, swapchain);
		swapchainImageViews = createSwapchainImageViews(device, swapchainImages, swapchainImageFormat);
		framebuffers = createFramebuffers(device, swapchainImageViews, renderPass, swapchainExtent);
	}

	void SetProjectionMatrix(matrix4x4 projectionMatrix) { 
		//Vulkan uses an inverted z axis
		projectionMatrix.m33 *= -1;
		projectionMatrix.m43 *= -1;

		uniformBufferObject.projection = projectionMatrix;
	}

	void SetFramebufferResized() {
		framebufferResized = true;
	}

private:
	struct QueueFamilyIndices {
		std::optional<uint32_t> graphicsFamily;
		std::optional<uint32_t> presentFamily;

		bool isComplete() {
			return graphicsFamily.has_value() && presentFamily.has_value();
		}
	};

	struct SwapchainSupportDetails {
		VkSurfaceCapabilitiesKHR capabilities;
		std::vector<VkSurfaceFormatKHR> formats;
		std::vector<VkPresentModeKHR> presentModes;
	};

	struct Vertex {
		vector2 pos;
		vector3 color;
		float texIndex;
		vector2 texCoord;

		static VkVertexInputBindingDescription getBindingDescription() {
			VkVertexInputBindingDescription bindingDescription{};
			bindingDescription.binding = 0;
			bindingDescription.stride = sizeof(Vertex);
			bindingDescription.inputRate = VK_VERTEX_INPUT_RATE_VERTEX;

			return bindingDescription;
		}

		static std::array<VkVertexInputAttributeDescription, 4> getAttributeDescriptions() {
			std::array<VkVertexInputAttributeDescription, 4> attributeDescriptions{};
			attributeDescriptions[0].binding = 0;
			attributeDescriptions[0].location = 0;
			attributeDescriptions[0].format = VK_FORMAT_R32G32_SFLOAT;
			attributeDescriptions[0].offset = offsetof(Vertex, pos);

			attributeDescriptions[1].binding = 0;
			attributeDescriptions[1].location = 1;
			attributeDescriptions[1].format = VK_FORMAT_R32G32B32_SFLOAT;
			attributeDescriptions[1].offset = offsetof(Vertex, color);

			attributeDescriptions[2].binding = 0;
			attributeDescriptions[2].location = 2;
			attributeDescriptions[2].format = VK_FORMAT_R32_SFLOAT;
			attributeDescriptions[2].offset = offsetof(Vertex, texIndex);

			attributeDescriptions[3].binding = 0;
			attributeDescriptions[3].location = 3;
			attributeDescriptions[3].format = VK_FORMAT_R32G32_SFLOAT;
			attributeDescriptions[3].offset = offsetof(Vertex, texCoord);

			return attributeDescriptions;
		}
	};

	struct UniformBufferObject {
		alignas(16) matrix4x4 projection;
	};

	HWND window;
	VkInstance vulkanInstance;
	VkSurfaceKHR targetSurface;
	VkPhysicalDevice physicalDevice;
	QueueFamilyIndices queueFamilies;
	SwapchainSupportDetails swapchainSupport;
	VkDevice device;
	VkQueue graphicsQueue;
	VkQueue presentQueue;
	VkSwapchainKHR swapchain;
	VkFormat swapchainImageFormat;
	VkExtent2D swapchainExtent;
	std::vector<VkImage> swapchainImages;
	std::vector<VkImageView> swapchainImageViews;
	VkPipelineLayout pipelineLayout;
	VkRenderPass renderPass;
	VkPipeline graphicsPipeline;
	std::vector<VkFramebuffer> framebuffers;
	VkCommandPool commandPool;
	VkCommandBuffer commandBuffer;
	VkDescriptorPool descriptorPool;
	VkDescriptorSetLayout descriptorSetLayout;
	VkDescriptorSet descriptorSet;
	VkSemaphore imageAvalibleSemaphore;
	VkSemaphore renderFinishedSemaphore;
	VkFence isRenderingFence;
	VkBuffer indexBuffer;
	VkDeviceMemory indexBufferMemory;
	VkBuffer vertexBuffer;
	VkDeviceMemory vertexBufferMemory;
	void* vertexBufferMapping;
	std::vector<Vertex> vertexBufferData;
	VkBuffer uniformBuffer;
	VkDeviceMemory uniformBufferMemory;
	void* uniformBufferMapping;
	VkImage textureImage;
	VkDeviceMemory textureImageMemory;
	VkImageView textureImageView;
	VkSampler textureSampler;

	bool framebufferResized = false;
	UniformBufferObject uniformBufferObject;
	uint32_t currentImageIndex;
	uint32_t currentVertexCount;
	uint32_t currentIndexCount;

	static VkInstance createVulkanInstance() {
		VkApplicationInfo applicationInfo{};
		applicationInfo.sType = VK_STRUCTURE_TYPE_APPLICATION_INFO;
		applicationInfo.pApplicationName = "Azalea Application";
		applicationInfo.applicationVersion = VK_MAKE_API_VERSION(0, 1, 0, 0);
		applicationInfo.pEngineName = "Azalea";
		applicationInfo.engineVersion = VK_MAKE_API_VERSION(0, 1, 0, 0);
		applicationInfo.apiVersion = VK_API_VERSION_1_0;

		VkInstanceCreateInfo instanceInfo{};
		instanceInfo.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
		instanceInfo.pApplicationInfo = &applicationInfo;
		instanceInfo.enabledExtensionCount = static_cast<uint32_t>(instanceExtentions.size());
		instanceInfo.ppEnabledExtensionNames = instanceExtentions.data();
		instanceInfo.enabledLayerCount = static_cast<uint32_t>(validationLayers.size());
		instanceInfo.ppEnabledLayerNames = validationLayers.data();

		VkInstance instance;
		if (vkCreateInstance(&instanceInfo, nullptr, &instance) != VK_SUCCESS) {
			THROW("Could not create vulkan instance!");
		}

		return instance;
	}

	static void destroyVulkanInstance(VkInstance instance) {
		vkDestroyInstance(instance, nullptr);
	}

	static bool checkPhysicalDeviceExtentionSupport(VkPhysicalDevice physicalDevice) {
		uint32_t extentionCount;
		vkEnumerateDeviceExtensionProperties(physicalDevice, nullptr, &extentionCount, nullptr);

		std::vector<VkExtensionProperties> avalibleExtentions(extentionCount);
		vkEnumerateDeviceExtensionProperties(physicalDevice, nullptr, &extentionCount, avalibleExtentions.data());

		std::set<std::string> requiredExtentions(deviceExtensions.begin(), deviceExtensions.end());

		for (const auto& extention : avalibleExtentions) {
			requiredExtentions.erase(extention.extensionName);
		}

		return requiredExtentions.empty();
	}

	static bool isPhysicalDeviceSuitable(VkPhysicalDevice physicalDevice, VkSurfaceKHR targetSurface) {
		QueueFamilyIndices indices = getQueueFamilies(physicalDevice, targetSurface);
		if (indices.isComplete() == false) return false;

		bool extentionsSupported = checkPhysicalDeviceExtentionSupport(physicalDevice);
		if (extentionsSupported == false) return false;

		SwapchainSupportDetails swapchainSupport = getSwapchainSupport(physicalDevice, targetSurface);
		bool swapchainAdequate = swapchainSupport.formats.empty() == false && swapchainSupport.presentModes.empty() == false;
		if (swapchainAdequate == false) return false;

		VkPhysicalDeviceFeatures supportedFeatures;
		vkGetPhysicalDeviceFeatures(physicalDevice, &supportedFeatures);
		if (supportedFeatures.samplerAnisotropy == false) return false;

		return true;
	}

	static VkPhysicalDevice selectPhysicalDevice(VkInstance instance, VkSurfaceKHR targetSurface) {
		uint32_t deviceCount;
		vkEnumeratePhysicalDevices(instance, &deviceCount, nullptr);

		if (deviceCount == 0) {
			THROW("Could not find GPU with vulkan support!");
		}

		std::vector<VkPhysicalDevice> devices(deviceCount);
		vkEnumeratePhysicalDevices(instance, &deviceCount, devices.data());

		std::optional<VkPhysicalDevice> physicalDevice;
		for (const auto& device : devices) {
			if (isPhysicalDeviceSuitable(device, targetSurface) == true) {
				physicalDevice = device;
				break;
			}
		}

		if (physicalDevice.has_value() == false) {
			THROW("Could not find suitable GPU!");
		}

		return physicalDevice.value();
	}

	static QueueFamilyIndices getQueueFamilies(VkPhysicalDevice device, VkSurfaceKHR targetSurface) {
		QueueFamilyIndices indices;

		uint32_t queueFamilyCount = 0;
		vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, nullptr);

		std::vector<VkQueueFamilyProperties> queueFamilies(queueFamilyCount);
		vkGetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, queueFamilies.data());

		for (uint32_t i = 0; i < queueFamilyCount; i++) {
			if (queueFamilies[i].queueFlags & VK_QUEUE_GRAPHICS_BIT) {
				indices.graphicsFamily = i;
			}

			VkBool32 presentSupport = false;
			vkGetPhysicalDeviceSurfaceSupportKHR(device, i, targetSurface, &presentSupport);
			if (presentSupport) {
				indices.presentFamily = i;
			}

			if (indices.isComplete()) break;
		}

		return indices;
	}

	static VkDevice createDevice(VkPhysicalDevice physicalDevice, QueueFamilyIndices queueFamilies) {
		std::vector<VkDeviceQueueCreateInfo> queueCreateInfos;
		std::set<uint32_t> uniqueQueueFamilies = { queueFamilies.graphicsFamily.value(), queueFamilies.presentFamily.value() };

		float queuePriority = 1.0f;
		for (uint32_t queueFamily : uniqueQueueFamilies) {
			VkDeviceQueueCreateInfo queueCreateInfo{};
			queueCreateInfo.sType = VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO;
			queueCreateInfo.queueFamilyIndex = queueFamily;
			queueCreateInfo.queueCount = 1;
			queueCreateInfo.pQueuePriorities = &queuePriority;
			queueCreateInfos.push_back(queueCreateInfo);
		}

		VkPhysicalDeviceFeatures deviceFeatures{};
		deviceFeatures.samplerAnisotropy = VK_TRUE;

		VkDeviceCreateInfo deviceInfo{};
		deviceInfo.sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO;
		deviceInfo.queueCreateInfoCount = static_cast<uint32_t>(queueCreateInfos.size());
		deviceInfo.pQueueCreateInfos = queueCreateInfos.data();
		deviceInfo.enabledExtensionCount = static_cast<uint32_t>(deviceExtensions.size());
		deviceInfo.ppEnabledExtensionNames = deviceExtensions.data();
		deviceInfo.pEnabledFeatures = &deviceFeatures;

		VkDevice device;
		if (vkCreateDevice(physicalDevice, &deviceInfo, nullptr, &device) != VK_SUCCESS) {
			THROW("Could not create virtual device!");
		}

		return device;
	}

	static void destroyDevice(VkDevice device) {
		vkDestroyDevice(device, nullptr);
	}

	static VkQueue getGraphicsQueue(VkDevice device, QueueFamilyIndices queueFamilies) {
		VkQueue graphicsQueue;
		vkGetDeviceQueue(device, queueFamilies.graphicsFamily.value(), 0, &graphicsQueue);

		return graphicsQueue;
	}

	static VkQueue getPresentQueue(VkDevice device, QueueFamilyIndices queueFamilies) {
		VkQueue presentQueue;
		vkGetDeviceQueue(device, queueFamilies.presentFamily.value(), 0, &presentQueue);

		return presentQueue;
	}

	static VkSurfaceKHR createWindowsSurface(VkInstance vulkanInstance, HWND window, HINSTANCE hInstance) {
		VkWin32SurfaceCreateInfoKHR surfaceInfo{};
		surfaceInfo.sType = VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR;
		surfaceInfo.hwnd = window;
		surfaceInfo.hinstance = hInstance;

		VkSurfaceKHR surface;
		if (vkCreateWin32SurfaceKHR(vulkanInstance, &surfaceInfo, nullptr, &surface) != VK_SUCCESS) {
			THROW("Could not create window surface!");
		}

		return surface;
	}

	static void destroySurface(VkInstance vulkanInstance, VkSurfaceKHR surface) {
		vkDestroySurfaceKHR(vulkanInstance, surface, nullptr);
	}

	static SwapchainSupportDetails getSwapchainSupport(VkPhysicalDevice physicalDevice, VkSurfaceKHR targetSurface) {
		SwapchainSupportDetails details;

		vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physicalDevice, targetSurface, &details.capabilities);

		uint32_t formatCount;
		vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, targetSurface, &formatCount, nullptr);

		if (formatCount != 0) {
			details.formats.resize(formatCount);
			vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, targetSurface, &formatCount, details.formats.data());
		}

		uint32_t presentModeCount;
		vkGetPhysicalDeviceSurfacePresentModesKHR(physicalDevice, targetSurface, &presentModeCount, nullptr);

		if (presentModeCount != 0) {
			details.presentModes.resize(presentModeCount);
			vkGetPhysicalDeviceSurfacePresentModesKHR(physicalDevice, targetSurface, &presentModeCount, details.presentModes.data());
		}

		return details;
	}

	static VkSurfaceFormatKHR chooseSwapSurfaceFormat(const std::vector<VkSurfaceFormatKHR>& avalibleFormats) {
		for (const auto& avalibleFormat : avalibleFormats) {
			if (avalibleFormat.format == VK_FORMAT_B8G8R8A8_SRGB || avalibleFormat.format == VK_COLOR_SPACE_SRGB_NONLINEAR_KHR) {
				return avalibleFormat;
			}
		}

		return avalibleFormats[0];
	}

	static VkPresentModeKHR chooseSwapPresentMode(const std::vector<VkPresentModeKHR>& availablePresentModes) {
		for (const auto& availablePresentMode : availablePresentModes) {
			if (availablePresentMode == VK_PRESENT_MODE_MAILBOX_KHR) {
				return availablePresentMode;
			}
		}

		return VK_PRESENT_MODE_FIFO_KHR;
	}

	static VkExtent2D chooseSwapExtent(const VkSurfaceCapabilitiesKHR& capabilities, HWND window) {
		if (capabilities.currentExtent.width != 0xFFFFFFFF) {
			return capabilities.currentExtent;
		}
		else {
			RECT clientRectiangle;
			if (GetClientRect(window, &clientRectiangle) == false) {
				THROW("Could not get window client rectangle!");
			}

			VkExtent2D actualExtent = {
				static_cast<uint32_t>(clientRectiangle.right),
				static_cast<uint32_t>(clientRectiangle.bottom)
			};

			std::cout << actualExtent.width;

			actualExtent.width = std::clamp(actualExtent.width, capabilities.minImageExtent.width, capabilities.maxImageExtent.width);
			actualExtent.height = std::clamp(actualExtent.height, capabilities.minImageExtent.height, capabilities.maxImageExtent.height);

			return actualExtent;
		}
	}

	static VkSwapchainKHR createSwapchain(VkDevice device, VkSurfaceKHR targetSurface, SwapchainSupportDetails swapchainSupport, QueueFamilyIndices queueFamilies, HWND window) {
		VkSurfaceFormatKHR surfaceFormat = chooseSwapSurfaceFormat(swapchainSupport.formats);
		VkPresentModeKHR presentMode = chooseSwapPresentMode(swapchainSupport.presentModes);
		VkExtent2D extent = chooseSwapExtent(swapchainSupport.capabilities, window);

		uint32_t imageCount = swapchainSupport.capabilities.minImageCount + 1;
		if (swapchainSupport.capabilities.maxImageCount > 0 && imageCount > swapchainSupport.capabilities.maxImageCount) {
			imageCount = swapchainSupport.capabilities.maxImageCount;
		}

		VkSwapchainCreateInfoKHR swapchainInfo{};
		swapchainInfo.sType = VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR;
		swapchainInfo.surface = targetSurface;
		swapchainInfo.minImageCount = imageCount;
		swapchainInfo.imageFormat = surfaceFormat.format;
		swapchainInfo.imageColorSpace = surfaceFormat.colorSpace;
		swapchainInfo.imageExtent = extent;
		swapchainInfo.imageArrayLayers = 1;
		swapchainInfo.imageUsage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT;

		
		if (queueFamilies.graphicsFamily != queueFamilies.presentFamily) {
			swapchainInfo.imageSharingMode = VK_SHARING_MODE_CONCURRENT;
			
			uint32_t queueFamilyIndices[] = { queueFamilies.graphicsFamily.value(), queueFamilies.presentFamily.value() };
			swapchainInfo.queueFamilyIndexCount = 2;
			swapchainInfo.pQueueFamilyIndices = queueFamilyIndices;
		}
		else {
			swapchainInfo.imageSharingMode = VK_SHARING_MODE_EXCLUSIVE;
		}

		swapchainInfo.preTransform = swapchainSupport.capabilities.currentTransform;
		swapchainInfo.compositeAlpha = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR;
		swapchainInfo.presentMode = presentMode;
		swapchainInfo.clipped = VK_TRUE;
		swapchainInfo.oldSwapchain = VK_NULL_HANDLE;

		VkSwapchainKHR swapchain;
		if (vkCreateSwapchainKHR(device, &swapchainInfo, nullptr, &swapchain) != VK_SUCCESS) {
			THROW("Could not create swap chain!");
		}

		return swapchain;
	}

	static void destroySwapchain(VkDevice device, VkSwapchainKHR swapchain) {
		vkDestroySwapchainKHR(device, swapchain, nullptr);
	}

	static std::vector<VkImage> getSwapchainImages(VkDevice device, VkSwapchainKHR swapchain) {
		uint32_t imageCount;
		vkGetSwapchainImagesKHR(device, swapchain, &imageCount, nullptr);

		std::vector<VkImage> swapchainImages(imageCount);
		vkGetSwapchainImagesKHR(device, swapchain, &imageCount, swapchainImages.data());

		return swapchainImages;
	}

	static VkImageView createImageView(VkDevice device, VkImage image, VkFormat format) {
		VkImageViewCreateInfo imageViewInfo{};
		imageViewInfo.sType = VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO;
		imageViewInfo.image = image;
		imageViewInfo.viewType = VK_IMAGE_VIEW_TYPE_2D;
		imageViewInfo.format = format;

		imageViewInfo.components.r = VK_COMPONENT_SWIZZLE_IDENTITY;
		imageViewInfo.components.g = VK_COMPONENT_SWIZZLE_IDENTITY;
		imageViewInfo.components.b = VK_COMPONENT_SWIZZLE_IDENTITY;
		imageViewInfo.components.a = VK_COMPONENT_SWIZZLE_IDENTITY;

		imageViewInfo.subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
		imageViewInfo.subresourceRange.baseMipLevel = 0;
		imageViewInfo.subresourceRange.levelCount = 1;
		imageViewInfo.subresourceRange.baseArrayLayer = 0;
		imageViewInfo.subresourceRange.layerCount = 1;

		VkImageView imageView;
		if (vkCreateImageView(device, &imageViewInfo, nullptr, &imageView) != VK_SUCCESS) {
			THROW("Could not create image view!");
		}

		return imageView;
	}

	static std::vector<VkImageView> createSwapchainImageViews(VkDevice device, std::vector<VkImage> images, VkFormat swapchainImageFormat) {
		std::vector<VkImageView> imageViews(images.size());

		for (size_t i = 0; i < images.size(); i++) {
			imageViews[i] = createImageView(device, images[i], swapchainImageFormat);
		}

		return imageViews;
	}

	static void destroyImageView(VkDevice device, VkImageView imageView) {
		vkDestroyImageView(device, imageView, nullptr);
	}

	static void destroyImageViews(VkDevice device, std::vector<VkImageView> imageViews) {
		for (auto imageView : imageViews) {
			destroyImageView(device, imageView);
		}
	}

	static void cleanupSwapchain(VkDevice device, std::vector<VkFramebuffer> framebuffers, std::vector<VkImageView> imageViews, VkSwapchainKHR swapchain) {
		destroyFramebuffers(device, framebuffers);
		destroyImageViews(device, imageViews);
		destroySwapchain(device, swapchain);
	}

	static VkShaderModule createShaderModule(VkDevice device, byte* shaderData, int shaderLength) {
		VkShaderModuleCreateInfo shaderModuleInfo{};
		shaderModuleInfo.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
		shaderModuleInfo.codeSize = static_cast<size_t>(shaderLength);
		shaderModuleInfo.pCode = reinterpret_cast<const uint32_t*>(shaderData);

		VkShaderModule shaderModule;
		if (vkCreateShaderModule(device, &shaderModuleInfo, nullptr, &shaderModule)) {
			THROW("Could not create shader module!");
		}

		return shaderModule;
	}

	static VkPipelineLayout createPipelineLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout) {
		VkPipelineLayoutCreateInfo pipelineLayoutInfo{};
		pipelineLayoutInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO;
		pipelineLayoutInfo.setLayoutCount = 1;
		pipelineLayoutInfo.pSetLayouts = &descriptorSetLayout;

		VkPipelineLayout pipelineLayout;
		if (vkCreatePipelineLayout(device, &pipelineLayoutInfo, nullptr, &pipelineLayout) != VK_SUCCESS) {
			THROW("Could not create pipeline layout!");
		}

		return pipelineLayout;
	}

	static void destroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout) {
		vkDestroyPipelineLayout(device, pipelineLayout, nullptr);
	}

	static VkRenderPass createRenderPass(VkDevice device, VkFormat swapchainImageFormat) {
		VkAttachmentDescription colorAttachment{};
		colorAttachment.format = swapchainImageFormat;
		colorAttachment.samples = VK_SAMPLE_COUNT_1_BIT;
		colorAttachment.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
		colorAttachment.storeOp = VK_ATTACHMENT_STORE_OP_STORE;
		colorAttachment.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
		colorAttachment.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
		colorAttachment.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
		colorAttachment.finalLayout = VK_IMAGE_LAYOUT_PRESENT_SRC_KHR;

		VkAttachmentReference colorAttachmentRef{};
		colorAttachmentRef.attachment = 0;
		colorAttachmentRef.layout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL;

		VkSubpassDescription subpass{};
		subpass.pipelineBindPoint = VK_PIPELINE_BIND_POINT_GRAPHICS;
		subpass.colorAttachmentCount = 1;
		subpass.pColorAttachments = &colorAttachmentRef;

		VkRenderPassCreateInfo renderPassInfo{};
		renderPassInfo.sType = VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO;
		renderPassInfo.attachmentCount = 1;
		renderPassInfo.pAttachments = &colorAttachment;
		renderPassInfo.subpassCount = 1;
		renderPassInfo.pSubpasses = &subpass;

		VkSubpassDependency subpassDependency{};
		subpassDependency.srcSubpass = VK_SUBPASS_EXTERNAL;
		subpassDependency.dstSubpass = 0;
		subpassDependency.srcStageMask = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT;
		subpassDependency.srcAccessMask = 0;
		subpassDependency.dstStageMask = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT;
		subpassDependency.dstAccessMask = VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT;

		renderPassInfo.dependencyCount = 1;
		renderPassInfo.pDependencies = &subpassDependency;

		VkRenderPass renderPass;
		if (vkCreateRenderPass(device, &renderPassInfo, nullptr, &renderPass) != VK_SUCCESS) {
			THROW("Could not create render pass!");
		}

		return renderPass;
	}

	static void destroyRenderPass(VkDevice device, VkRenderPass renderPass) {
		vkDestroyRenderPass(device, renderPass, nullptr);
	}

	static VkPipeline createGraphicsPipeline(VkDevice device, VkPipelineLayout pipelineLayout, VkRenderPass renderPass, byte* vertShaderData, int vertShaderLength, byte* fragShaderData, int fragShaderLength, VkExtent2D swapchainExtent) {
		VkShaderModule vertShaderModule = createShaderModule(device, vertShaderData, vertShaderLength);
		VkShaderModule fragShaderModule = createShaderModule(device, fragShaderData, fragShaderLength);

		VkPipelineShaderStageCreateInfo shaderStages[] = { {}, {} };
		shaderStages[0].sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
		shaderStages[0].stage = VK_SHADER_STAGE_VERTEX_BIT;
		shaderStages[0].module = vertShaderModule;
		shaderStages[0].pName = "main";

		shaderStages[1].sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
		shaderStages[1].stage = VK_SHADER_STAGE_FRAGMENT_BIT;
		shaderStages[1].module = fragShaderModule;
		shaderStages[1].pName = "main";

		VkPipelineDynamicStateCreateInfo dynamicStateInfo{};
		dynamicStateInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO;
		dynamicStateInfo.dynamicStateCount = static_cast<uint32_t>(dynamicStates.size());
		dynamicStateInfo.pDynamicStates = dynamicStates.data();

		auto bindingDescription = Vertex::getBindingDescription();
		auto attributeDescriptions = Vertex::getAttributeDescriptions();

		VkPipelineVertexInputStateCreateInfo vertexInputInfo{};
		vertexInputInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO;
		vertexInputInfo.vertexBindingDescriptionCount = 1;
		vertexInputInfo.pVertexBindingDescriptions = &bindingDescription;
		vertexInputInfo.vertexAttributeDescriptionCount = static_cast<uint32_t>(attributeDescriptions.size());
		vertexInputInfo.pVertexAttributeDescriptions = attributeDescriptions.data();

		VkPipelineInputAssemblyStateCreateInfo inputAssemblyInfo{};
		inputAssemblyInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO;
		inputAssemblyInfo.topology = VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST;
		inputAssemblyInfo.primitiveRestartEnable = VK_FALSE;

		VkPipelineViewportStateCreateInfo viewportStateInfo{};
		viewportStateInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO;
		viewportStateInfo.viewportCount = 1;
		viewportStateInfo.scissorCount = 1;

		VkViewport viewport{};
		viewport.x = 0.0f;
		viewport.y = 0.0f;
		viewport.width = static_cast<float>(swapchainExtent.width);
		viewport.height = static_cast<float>(swapchainExtent.height);
		viewport.minDepth = 0.0f;
		viewport.maxDepth = 1.0f;

		VkRect2D scissor{};
		scissor.offset = { 0, 0 };
		scissor.extent = swapchainExtent;

		VkPipelineRasterizationStateCreateInfo rasterizerInfo{};
		rasterizerInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO;
		rasterizerInfo.depthClampEnable = VK_FALSE;
		rasterizerInfo.rasterizerDiscardEnable = VK_FALSE;
		rasterizerInfo.polygonMode = VK_POLYGON_MODE_FILL;
		rasterizerInfo.lineWidth = 1.0f;
		rasterizerInfo.cullMode = VK_CULL_MODE_NONE;//VK_CULL_MODE_BACK_BIT;
		rasterizerInfo.frontFace = VK_FRONT_FACE_CLOCKWISE;
		rasterizerInfo.depthBiasEnable = VK_FALSE;

		VkPipelineMultisampleStateCreateInfo multisamplingInfo{};
		multisamplingInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO;
		multisamplingInfo.sampleShadingEnable = VK_FALSE;
		multisamplingInfo.rasterizationSamples = VK_SAMPLE_COUNT_1_BIT;

		VkPipelineColorBlendAttachmentState colorAttachmentInfo{};
		colorAttachmentInfo.colorWriteMask = VK_COLOR_COMPONENT_R_BIT | VK_COLOR_COMPONENT_G_BIT | VK_COLOR_COMPONENT_B_BIT | VK_COLOR_COMPONENT_A_BIT;
		colorAttachmentInfo.blendEnable = VK_TRUE;
		colorAttachmentInfo.srcColorBlendFactor = VK_BLEND_FACTOR_SRC_ALPHA;
		colorAttachmentInfo.dstColorBlendFactor = VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA;
		colorAttachmentInfo.colorBlendOp = VK_BLEND_OP_ADD;
		colorAttachmentInfo.srcAlphaBlendFactor = VK_BLEND_FACTOR_ONE;
		colorAttachmentInfo.dstAlphaBlendFactor = VK_BLEND_FACTOR_ZERO;
		colorAttachmentInfo.alphaBlendOp = VK_BLEND_OP_ADD;

		VkPipelineColorBlendStateCreateInfo colorBlendingInfo{};
		colorBlendingInfo.sType = VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
		colorBlendingInfo.logicOpEnable = VK_FALSE;
		colorBlendingInfo.attachmentCount = 1;
		colorBlendingInfo.pAttachments = &colorAttachmentInfo;

		VkGraphicsPipelineCreateInfo pipelineInfo{};
		pipelineInfo.sType = VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO;
		pipelineInfo.stageCount = 2;
		pipelineInfo.pStages = shaderStages;
		pipelineInfo.pVertexInputState = &vertexInputInfo;
		pipelineInfo.pInputAssemblyState = &inputAssemblyInfo;
		pipelineInfo.pViewportState = &viewportStateInfo;
		pipelineInfo.pRasterizationState = &rasterizerInfo;
		pipelineInfo.pMultisampleState = &multisamplingInfo;
		pipelineInfo.pDepthStencilState = nullptr;
		pipelineInfo.pColorBlendState = &colorBlendingInfo;
		pipelineInfo.pDynamicState = &dynamicStateInfo;
		pipelineInfo.layout = pipelineLayout;
		pipelineInfo.renderPass = renderPass;
		pipelineInfo.subpass = 0;

		VkPipeline graphicsPipeline;
		if (vkCreateGraphicsPipelines(device, VK_NULL_HANDLE, 1, &pipelineInfo, nullptr, &graphicsPipeline) != VK_SUCCESS) {
			THROW("Could not create graphics pipeline!");
		}

		vkDestroyShaderModule(device, fragShaderModule, nullptr);
		vkDestroyShaderModule(device, vertShaderModule, nullptr);

		return graphicsPipeline;
	}

	static void destroyGraphicsPipeline(VkDevice device, VkPipeline graphicsPipeline) {
		vkDestroyPipeline(device, graphicsPipeline, nullptr);
	}

	static std::vector<VkFramebuffer> createFramebuffers(VkDevice device, std::vector<VkImageView> swapchainImageViews, VkRenderPass renderPass, VkExtent2D swapchainExtent) {
		std::vector<VkFramebuffer> framebuffers(swapchainImageViews.size());

		for (size_t i = 0; i < swapchainImageViews.size(); i++) {
			VkImageView attachments[] = { swapchainImageViews[i] };

			VkFramebufferCreateInfo framebufferInfo{};
			framebufferInfo.sType = VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO;
			framebufferInfo.renderPass = renderPass;
			framebufferInfo.attachmentCount = 1;
			framebufferInfo.pAttachments = attachments;
			framebufferInfo.width = swapchainExtent.width;
			framebufferInfo.height = swapchainExtent.height;
			framebufferInfo.layers = 1;

			if (vkCreateFramebuffer(device, &framebufferInfo, nullptr, &framebuffers[i]) != VK_SUCCESS) {
				THROW("Could not create framebuffer!");
			}
		}

		return framebuffers;
	}

	static void destroyFramebuffers(VkDevice device, std::vector<VkFramebuffer> framebuffers) {
		for (auto framebuffer : framebuffers) {
			vkDestroyFramebuffer(device, framebuffer, nullptr);
		}
	}

	static VkCommandPool createCommandPool(VkDevice device, QueueFamilyIndices queueFamilies) {
		VkCommandPoolCreateInfo commandPoolInfo{};
		commandPoolInfo.sType = VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO;
		commandPoolInfo.flags = VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT;
		commandPoolInfo.queueFamilyIndex = queueFamilies.graphicsFamily.value();

		VkCommandPool commandPool;
		if (vkCreateCommandPool(device, &commandPoolInfo, nullptr, &commandPool) != VK_SUCCESS) {
			THROW("Could not create command pool!");
		}

		return commandPool;
	}

	static void destroyCommandPool(VkDevice device, VkCommandPool commandPool) {
		vkDestroyCommandPool(device, commandPool, nullptr);
	}

	static VkCommandBuffer createCommandBuffer(VkDevice device, VkCommandPool commandPool) {
		VkCommandBufferAllocateInfo commandBufferInfo{};
		commandBufferInfo.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO;
		commandBufferInfo.commandPool = commandPool;
		commandBufferInfo.level = VK_COMMAND_BUFFER_LEVEL_PRIMARY;
		commandBufferInfo.commandBufferCount = 1;

		VkCommandBuffer commandBuffer;
		if (vkAllocateCommandBuffers(device, &commandBufferInfo, &commandBuffer) != VK_SUCCESS) {
			THROW("Could not create command buffer!");
		}

		return commandBuffer;
	}

	static VkSemaphore createSemaphore(VkDevice device) {
		VkSemaphoreCreateInfo semaphoreInfo{};
		semaphoreInfo.sType = VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO;

		VkSemaphore semaphore;
		if (vkCreateSemaphore(device, &semaphoreInfo, nullptr, &semaphore) != VK_SUCCESS) {
			THROW("Could not create semaphore!");
		}

		return semaphore;
	}

	static void destroySemaphore(VkDevice device, VkSemaphore semaphore) {
		vkDestroySemaphore(device, semaphore, nullptr);
	}

	static VkFence createFence(VkDevice device, bool isSignaled) {
		VkFenceCreateInfo fenceInfo{};
		fenceInfo.sType = VK_STRUCTURE_TYPE_FENCE_CREATE_INFO;

		if (isSignaled) {
			fenceInfo.flags = VK_FENCE_CREATE_SIGNALED_BIT;
		}

		VkFence fence;
		if (vkCreateFence(device, &fenceInfo, nullptr, &fence) != VK_SUCCESS) {
			THROW("Could not create fence!");
		}

		return fence;
	}

	static void destroyFence(VkDevice device, VkFence fence) {
		vkDestroyFence(device, fence, nullptr);
	}

	static VkBuffer createBuffer(VkDevice device, VkDeviceSize size, VkBufferUsageFlags usage) {
		VkBufferCreateInfo bufferInfo{};
		bufferInfo.sType = VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO;
		bufferInfo.size = size;
		bufferInfo.usage = usage;
		bufferInfo.sharingMode = VK_SHARING_MODE_EXCLUSIVE;

		VkBuffer buffer;
		if (vkCreateBuffer(device, &bufferInfo, nullptr, &buffer) != VK_SUCCESS) {
			THROW("Could not create buffer!");
		}

		return buffer;
	}

	static void destroyBuffer(VkDevice device, VkBuffer buffer) {
		vkDestroyBuffer(device, buffer, nullptr);
	}

	static VkDescriptorSetLayout createDescriptorSetLayout(VkDevice device) {
		VkDescriptorSetLayoutBinding uboLayoutBinding{};
		uboLayoutBinding.binding = 0;
		uboLayoutBinding.descriptorType = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;;
		uboLayoutBinding.descriptorCount = 1;
		uboLayoutBinding.stageFlags = VK_SHADER_STAGE_VERTEX_BIT;

		VkDescriptorSetLayoutBinding samplerLayoutBinding{};
		samplerLayoutBinding.binding = 1;
		samplerLayoutBinding.descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
		samplerLayoutBinding.descriptorCount = 1;
		samplerLayoutBinding.pImmutableSamplers = nullptr;
		samplerLayoutBinding.stageFlags = VK_SHADER_STAGE_FRAGMENT_BIT;

		std::array<VkDescriptorSetLayoutBinding, 2> bindings = { uboLayoutBinding, samplerLayoutBinding };
		VkDescriptorSetLayoutCreateInfo layoutInfo{};
		layoutInfo.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO;
		layoutInfo.bindingCount = static_cast<uint32_t>(bindings.size());
		layoutInfo.pBindings = bindings.data();

		VkDescriptorSetLayout descriptorSetLayout;
		if (vkCreateDescriptorSetLayout(device, &layoutInfo, nullptr, &descriptorSetLayout) != VK_SUCCESS) {
			THROW("Could not create descriptor set layout!");
		}

		return descriptorSetLayout;
	}

	static void destroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout) {
		vkDestroyDescriptorSetLayout(device, descriptorSetLayout, nullptr);
	}

	static VkDescriptorPool createDescriptorPool(VkDevice device) {
		std::array<VkDescriptorPoolSize, 2> poolSizes{};
		poolSizes[0].type = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
		poolSizes[0].descriptorCount = 1;
		poolSizes[1].type = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
		poolSizes[1].descriptorCount = 1;

		VkDescriptorPoolCreateInfo descriptorPoolInfo{};
		descriptorPoolInfo.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO;
		descriptorPoolInfo.poolSizeCount = static_cast<int32_t>(poolSizes.size());
		descriptorPoolInfo.pPoolSizes = poolSizes.data();
		descriptorPoolInfo.maxSets = 1;

		VkDescriptorPool descriptorPool;
		if (vkCreateDescriptorPool(device, &descriptorPoolInfo, nullptr, &descriptorPool) != VK_SUCCESS) {
			THROW("Could not create descriptor pool!");
		}

		return descriptorPool;
	}

	static void destroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool) {
		vkDestroyDescriptorPool(device, descriptorPool, nullptr);
	}

	static VkDescriptorSet createDescriptorSet(VkDevice device, VkDescriptorPool descriptorPool, VkDescriptorSetLayout descriptorSetLayout) {
		VkDescriptorSetAllocateInfo allocateInfo{};
		allocateInfo.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO;
		allocateInfo.descriptorPool = descriptorPool;
		allocateInfo.descriptorSetCount = 1;
		allocateInfo.pSetLayouts = &descriptorSetLayout;

		VkDescriptorSet descriptorSet;
		if (vkAllocateDescriptorSets(device, &allocateInfo, &descriptorSet) != VK_SUCCESS) {
			THROW("Could not allocate descriptor sets");
		}

		return descriptorSet;
	}

	static VkImage createTextureImage(VkDevice device, int textureWidth, int textureHeight, int textureChannels, void* textureData) {
		VkDeviceSize imageSize = textureWidth * textureHeight * 4;

		VkImageCreateInfo imageInfo{};
		imageInfo.sType = VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO;
		imageInfo.imageType = VK_IMAGE_TYPE_2D;
		imageInfo.extent.width = static_cast<uint32_t>(textureWidth);
		imageInfo.extent.height = static_cast<uint32_t>(textureHeight);
		imageInfo.extent.depth = 1;
		imageInfo.mipLevels = 1;
		imageInfo.arrayLayers = 1;
		imageInfo.format = VK_FORMAT_R8G8B8A8_SRGB;
		imageInfo.tiling = VK_IMAGE_TILING_OPTIMAL;
		imageInfo.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
		imageInfo.usage = VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT;
		imageInfo.sharingMode = VK_SHARING_MODE_EXCLUSIVE;
		imageInfo.samples = VK_SAMPLE_COUNT_1_BIT;


		VkImage image;
		if (vkCreateImage(device, &imageInfo, nullptr, &image) != VK_SUCCESS) {
			THROW("Could not create image!");
		}

		return image;
	}

	static void destroyImage(VkDevice device, VkImage image) {
		vkDestroyImage(device, image, nullptr);
	}

	static VkSampler createTextureSampler(VkPhysicalDevice physicalDevice, VkDevice device) {
		VkSamplerCreateInfo samplerInfo{};
		samplerInfo.sType = VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO;
		samplerInfo.magFilter = VK_FILTER_LINEAR;
		samplerInfo.minFilter = VK_FILTER_LINEAR;
		samplerInfo.addressModeU = VK_SAMPLER_ADDRESS_MODE_REPEAT;
		samplerInfo.addressModeV = VK_SAMPLER_ADDRESS_MODE_REPEAT;
		samplerInfo.addressModeW = VK_SAMPLER_ADDRESS_MODE_REPEAT;
		samplerInfo.borderColor = VK_BORDER_COLOR_INT_OPAQUE_BLACK;
		samplerInfo.unnormalizedCoordinates = VK_FALSE;
		samplerInfo.compareEnable = VK_FALSE;
		samplerInfo.compareOp = VK_COMPARE_OP_ALWAYS;
		samplerInfo.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
		samplerInfo.mipLodBias = 0.0f;
		samplerInfo.minLod = 0.0f;
		samplerInfo.maxLod = 0.0f;

		VkPhysicalDeviceProperties properties{};
		vkGetPhysicalDeviceProperties(physicalDevice, &properties);
		samplerInfo.anisotropyEnable = VK_TRUE;
		samplerInfo.maxAnisotropy = properties.limits.maxSamplerAnisotropy;

		VkSampler textureSampler;
		if (vkCreateSampler(device, &samplerInfo, nullptr, &textureSampler) != VK_SUCCESS) {
			THROW("Could not create texture sampler!");
		}

		return textureSampler;
	}

	static void destroySampler(VkDevice device, VkSampler sampler) {
		vkDestroySampler(device, sampler, nullptr);
	}

	static uint32_t getMemoryType(VkPhysicalDevice physicalDevice, uint32_t typeFilter, VkMemoryPropertyFlags properties) {
		VkPhysicalDeviceMemoryProperties memoryProperties;
		vkGetPhysicalDeviceMemoryProperties(physicalDevice, &memoryProperties);

		for (uint32_t i = 0; i < memoryProperties.memoryTypeCount; i++) {
			bool typeSuitable = typeFilter & (1 << i);
			bool propertiesSuitable = (memoryProperties.memoryTypes[i].propertyFlags & properties) == properties;

			if (typeSuitable && propertiesSuitable) {
				return i;
			}
		}

		THROW("Could not get suitable memory type!");
	}

	static VkDeviceMemory allocateImageMemory(VkPhysicalDevice physicalDevice, VkDevice device, VkImage image) {
		VkMemoryRequirements memoryRequirements;
		vkGetImageMemoryRequirements(device, image, &memoryRequirements);

		VkMemoryAllocateInfo allocateInfo{};
		allocateInfo.sType = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO;
		allocateInfo.allocationSize = memoryRequirements.size;
		allocateInfo.memoryTypeIndex = getMemoryType(physicalDevice, memoryRequirements.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);
	
		VkDeviceMemory imageMemory;
		if (vkAllocateMemory(device, &allocateInfo, nullptr, &imageMemory) != VK_SUCCESS) {
			THROW("Could not allocate image memory!");
		}

		if (vkBindImageMemory(device, image, imageMemory, 0) != VK_SUCCESS) {
			THROW("Could not bind memory to image!");
		};

		return imageMemory;
	}

	static VkDeviceMemory allocateBufferMemory(VkPhysicalDevice physicalDevice, VkDevice device, VkBuffer buffer, VkMemoryPropertyFlags properties) {
		VkMemoryRequirements memoryRequirements;
		vkGetBufferMemoryRequirements(device, buffer, &memoryRequirements);

		VkMemoryAllocateInfo allocateInfo{};
		allocateInfo.sType = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO;
		allocateInfo.allocationSize = memoryRequirements.size;
		allocateInfo.memoryTypeIndex = getMemoryType(physicalDevice, memoryRequirements.memoryTypeBits, properties);
		
		VkDeviceMemory bufferMemory;
		if (vkAllocateMemory(device, &allocateInfo, nullptr, &bufferMemory) != VK_SUCCESS) {
			THROW("Could not allocate buffer memory!");
		}
		
		if (vkBindBufferMemory(device, buffer, bufferMemory, 0) != VK_SUCCESS) {
			THROW("Could not bind buffer memory to buffer!");
		};

		return bufferMemory;
	}

	static void freeDeviceMemory(VkDevice device, VkDeviceMemory memory) {
		vkFreeMemory(device, memory, nullptr);
	}

	static VkCommandBuffer beginSingleTimeCommands(VkDevice device, VkCommandPool commandPool) {
		VkCommandBuffer commandBuffer = createCommandBuffer(device, commandPool);

		VkCommandBufferBeginInfo beginInfo{};
		beginInfo.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO;
		beginInfo.flags = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT;

		if (vkBeginCommandBuffer(commandBuffer, &beginInfo) != VK_SUCCESS) {
			THROW("Could not begin recording command buffer!");
		}

		return commandBuffer;
	}

	static VkCommandBuffer finishSingleTimeCommands(VkDevice device, VkCommandPool commandPool, VkQueue graphicsQueue, VkCommandBuffer commandBuffer) {
		if (vkEndCommandBuffer(commandBuffer) != VK_SUCCESS) {
			THROW("Failed to record framebuffer!");
		}

		VkSubmitInfo submitInfo{};
		submitInfo.sType = VK_STRUCTURE_TYPE_SUBMIT_INFO;
		submitInfo.commandBufferCount = 1;
		submitInfo.pCommandBuffers = &commandBuffer;

		if (vkQueueSubmit(graphicsQueue, 1, &submitInfo, VK_NULL_HANDLE) != VK_SUCCESS) {
			THROW("Could not submit copy command buffer!");
		}

		vkQueueWaitIdle(graphicsQueue);

		vkFreeCommandBuffers(device, commandPool, 1, &commandBuffer);
	}

	static void copyBuffer(VkDevice device, VkCommandPool commandPool, VkQueue graphicsQueue, VkBuffer source, VkBuffer destination, VkDeviceSize size) {
		VkCommandBuffer commandBuffer = beginSingleTimeCommands(device, commandPool);

		VkBufferCopy copyRegion{};
		copyRegion.srcOffset = 0;
		copyRegion.dstOffset = 0;
		copyRegion.size = size;

		vkCmdCopyBuffer(commandBuffer, source, destination, 1, &copyRegion);

		finishSingleTimeCommands(device, commandPool, graphicsQueue, commandBuffer);
	}

	static void copyBufferToImage(VkDevice device, VkCommandPool commandPool, VkQueue graphicsQueue, VkBuffer buffer, VkImage image, uint32_t width, uint32_t height) {
		VkCommandBuffer commandBuffer = beginSingleTimeCommands(device, commandPool);

		VkBufferImageCopy region{};
		region.bufferOffset = 0;
		region.bufferRowLength = 0;
		region.bufferImageHeight = 0;

		region.imageSubresource.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
		region.imageSubresource.mipLevel = 0;
		region.imageSubresource.baseArrayLayer = 0;
		region.imageSubresource.layerCount = 1;

		region.imageOffset = { 0, 0, 0 };
		region.imageExtent = { width, height, 1 };

		vkCmdCopyBufferToImage(commandBuffer, buffer, image, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, 1, &region);

		finishSingleTimeCommands(device, commandPool, graphicsQueue, commandBuffer);
	}

	static void transitionImageLayout(VkDevice device, VkCommandPool commandPool, VkQueue graphicsQueue, VkImage image, VkFormat format, VkImageLayout oldLayout, VkImageLayout newLayout) {
		VkCommandBuffer commandBuffer = beginSingleTimeCommands(device, commandPool);

		VkImageMemoryBarrier barrier{};
		barrier.sType = VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER;
		barrier.oldLayout = oldLayout;
		barrier.newLayout = newLayout;
		barrier.srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED;
		barrier.dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED;
		barrier.image = image;
		barrier.subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
		barrier.subresourceRange.baseMipLevel = 0;
		barrier.subresourceRange.levelCount = 1;
		barrier.subresourceRange.baseArrayLayer = 0;
		barrier.subresourceRange.layerCount = 1;

		VkPipelineStageFlags sourceStage;
		VkPipelineStageFlags destinationStage;

		if (oldLayout == VK_IMAGE_LAYOUT_UNDEFINED && newLayout == VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL) {
			barrier.srcAccessMask = 0;
			barrier.dstAccessMask = VK_ACCESS_TRANSFER_WRITE_BIT;

			sourceStage = VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;
			destinationStage = VK_PIPELINE_STAGE_TRANSFER_BIT;
		}
		else if (oldLayout == VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL && newLayout == VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL) {
			barrier.srcAccessMask = VK_ACCESS_TRANSFER_WRITE_BIT;
			barrier.dstAccessMask = VK_ACCESS_SHADER_READ_BIT;

			sourceStage = VK_PIPELINE_STAGE_TRANSFER_BIT;
			destinationStage = VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT;
		}
		else {
			THROW("Layout transition not supported!");
		}

		vkCmdPipelineBarrier(commandBuffer, sourceStage, destinationStage, 0, 0, nullptr, 0, nullptr, 1, &barrier);

		finishSingleTimeCommands(device, commandPool, graphicsQueue, commandBuffer);
	}

	static void configureUniformDescriptor(VkDevice device, VkBuffer uniformBuffer, VkImageView textureImageView, VkSampler textureSampler, VkDescriptorSet descriptorSet) {
		VkDescriptorBufferInfo bufferInfo{};
		bufferInfo.buffer = uniformBuffer;
		bufferInfo.offset = 0;
		bufferInfo.range = sizeof(UniformBufferObject);

		VkDescriptorImageInfo imageInfo{};
		imageInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
		imageInfo.imageView = textureImageView;
		imageInfo.sampler = textureSampler;

		std::array<VkWriteDescriptorSet, 2> descriptorWrites{};
		descriptorWrites[0].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		descriptorWrites[0].dstSet = descriptorSet;
		descriptorWrites[0].dstBinding = 0;
		descriptorWrites[0].dstArrayElement = 0;
		descriptorWrites[0].descriptorType = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
		descriptorWrites[0].descriptorCount = 1;
		descriptorWrites[0].pBufferInfo = &bufferInfo;

		descriptorWrites[1].sType = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET;
		descriptorWrites[1].dstSet = descriptorSet;
		descriptorWrites[1].dstBinding = 1;
		descriptorWrites[1].dstArrayElement = 0;
		descriptorWrites[1].descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
		descriptorWrites[1].descriptorCount = 1;
		descriptorWrites[1].pImageInfo = &imageInfo;

		vkUpdateDescriptorSets(device, static_cast<uint32_t>(descriptorWrites.size()), descriptorWrites.data(), 0, nullptr);
	}

	static VkResult presentSwapchain(VkQueue presentQueue, VkSwapchainKHR swapchain, uint32_t imageIndex, VkSemaphore renderFinishedSemaphore) {
		VkSemaphore signalSemaphores[] = { renderFinishedSemaphore };

		VkPresentInfoKHR presentInfo{};
		presentInfo.sType = VK_STRUCTURE_TYPE_PRESENT_INFO_KHR;
		presentInfo.waitSemaphoreCount = 1;
		presentInfo.pWaitSemaphores = signalSemaphores;

		VkSwapchainKHR swapchains[] = { swapchain };
		presentInfo.swapchainCount = 1;
		presentInfo.pSwapchains = swapchains;
		presentInfo.pImageIndices = &imageIndex;

		return vkQueuePresentKHR(presentQueue, &presentInfo);
	}

	static void updateUniformBuffer(VkExtent2D swapchainExtent, void* uniformBufferMapping, UniformBufferObject uniformBufferObject) {
		memcpy(uniformBufferMapping, &uniformBufferObject, sizeof(uniformBufferObject));
	}

	static void updateVertexBuffer(void* vertexBufferMapping, Vertex* vertexBufferData, size_t dataSize) {
		memcpy(vertexBufferMapping, vertexBufferData, dataSize);
	}
};

extern "C" __declspec(dllexport) VulkanController* CreateVulkanController(HWND window, HINSTANCE hInstance, byte* vertShaderData, int vertShaderLength, byte* fragShaderData, int fragShaderLength) {
	return new VulkanController(window, hInstance, vertShaderData, vertShaderLength, fragShaderData, fragShaderLength);
}

extern "C" __declspec(dllexport) void DestroyVulkanController(VulkanController* controller) { 
	delete controller; 
}

extern "C" __declspec(dllexport) void VulkanCreateTexture(VulkanController* controller, int width, int height, int channelCount, void* data) {
	controller->CreateTexture(width, height, channelCount, data);
}

extern "C" __declspec(dllexport) void VulkanBeginFrame(VulkanController* controller) {
	controller->BeginFrame();
}

extern "C" __declspec(dllexport) void VulkanFinishFrame(VulkanController* controller) {
	controller->FinishFrame();
}

extern "C" __declspec(dllexport) void VulkanPushQuad(VulkanController* controller, vector2 topLeft, vector2 topRight, vector2 bottomRight, vector2 bottomLeft) {
	controller->PushQuad(topLeft, topRight, bottomRight, bottomLeft);
}

extern "C" __declspec(dllexport) void VulkanSetProjectionMatrix(VulkanController* controller, matrix4x4 projectionMatrix) {
	controller->SetProjectionMatrix(projectionMatrix);
}

extern "C" __declspec(dllexport) void VulkanSetFramebufferResized(VulkanController* controller) {
	controller->SetFramebufferResized();
}

