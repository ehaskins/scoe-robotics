/*
 * StreamReader.h
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#ifndef STREAMREADER_H_
#define STREAMREADER_H_
#include <stdint.h>
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
unsigned char readUInt8(uint8_t data[], uint16_t * offset);
unsigned short readUInt16(uint8_t data[], uint16_t * offset);
unsigned long readUInt32(uint8_t data[], uint16_t * offset);
unsigned long long readUInt64(uint8_t data[], uint16_t * offset);
void readBytes(uint8_t data[], uint8_t out[], int count,
		int *offset);

#endif /* STREAMREADER_H_ */
