const canvas = document.getElementById("Canvas");
const gl = canvas.getContext("webgl2", { antialias: false, alpha: false });
var lastWindowWidth = 0;
var lastWindowHeight = 0;

const audio = new AudioContext();
const masterVolumeGain = audio.createGain();
masterVolumeGain.connect(audio.destination);

// WebEvents
const CheckClientSize = () => {
    if (lastWindowWidth == window.innerWidth && lastWindowHeight == window.innerHeight)
        return;

    canvas.width = lastWindowWidth = window.innerWidth;
    canvas.height = lastWindowHeight = window.innerHeight;
    UpdateClientSize(window.innerWidth, window.innerHeight);
}

// WebAudio
const BufferAudioData = (buffer, data) => {
    var uInt8Array = data._unsafe_create_view();
    var int16Array = new Int16Array(uInt8Array.buffer, uInt8Array.byteOffset, uInt8Array.length / 2);

    var channelDataPointers = new Array(buffer.numberOfChannels);
    for (let i = 0; i < buffer.numberOfChannels; i++) {
        channelDataPointers[i] = buffer.getChannelData(i);
    }

    var c = 1 / 32768;
    
    for (let i = 0; i < int16Array.length / buffer.numberOfChannels; i++) {
        for (var j = 0; j < buffer.numberOfChannels; j++) {
            channelDataPointers[j][i] = int16Array[(i * buffer.numberOfChannels) + j] * c;
        }
    }
}