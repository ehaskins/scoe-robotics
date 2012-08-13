/*
 * ByteWriter.h
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#ifndef BYTEWRITER_H_
#define BYTEWRITER_H_

int writeByte(unsigned char data[], unsigned char val, unsigned int offset);
int writeUInt16(unsigned char data[], unsigned short val, unsigned int offset);
int writeUInt32(unsigned char data[], unsigned long val, unsigned int offset);
int writeUInt32ForCrc(unsigned char data[], unsigned long val, unsigned int offset);
int writeBytes(unsigned char data[], unsigned int dataOffset, unsigned char val[], int count, unsigned int valOffset);
#endif /* BYTEWRITER_H_ */
