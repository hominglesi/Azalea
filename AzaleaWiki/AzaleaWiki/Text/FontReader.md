#Description

Binary reader specialized for reading TTF files.

TTF files are stored as big endian so when reading them the values need to be read in reverse. 

#Methods

void **SkipBytes**(int bytes);

**bytes** - Specifies the number of bytes to skip

Seeks forward by the specified amount of bytes.


void **ReadUInt16**();

Reads a 2-byte unsigned integer from the current stream using big-endian encoding and advances the position of the stream by two bytes.


void **ReadUInt32**();

Reads a 4-byte unsigned integer from the current stream using big-endian encoding and advances the position of the stream by four bytes.


void **ReadTag**();

Reads a 4 character string from the current stream and advances the position of the stream by 4 bytes.