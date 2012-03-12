/*
 * StreamReader.h
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#ifndef STREAMREADER_H_
#define STREAMREADER_H_

/*class StreamReader
{
private:
	unsigned char *data;
	int offset;
	int length;
public:
	StreamReader(unsigned char *, int);
	int ReadBytes(int, unsigned char *);
	unsigned char ReadUInt8(void);
	unsigned short ReadUInt16(void);
	unsigned int ReadUInt32(void);
	unsigned long ReadUInt64(void);
};*/
//void ReadBytes(unsigned char[], int*, int, unsigned char[]);
unsigned char readUInt8(unsigned char [], int*);
unsigned short readUInt16(unsigned char[], int*);
unsigned long readUInt32(unsigned char[], int*);
unsigned long long readUInt64(unsigned char[], int*);
void readBytes(unsigned char data[], unsigned char out[], int count, int *offset);

#endif /* STREAMREADER_H_ */
