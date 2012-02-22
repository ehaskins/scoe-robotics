/*
 * ByteWriter.h
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#ifndef BYTEWRITER_H_
#define BYTEWRITER_H_
#include <stdint.h>
int writeByte(uint8_t data[], uint8_t val, uint16_t offset);
int writeUInt16(uint8_t data[], uint16_t val, uint16_t offset);
int writeUInt32(uint8_t data[], uint32_t val, uint16_t offset);
int writeUInt32ForCrc(uint8_t data[], uint32_t val, uint16_t offset);
int writeBytes(uint8_t data[], uint16_t dataOffset, uint8_t val[], uint16_t count, uint16_t valOffset);
#endif /* BYTEWRITER_H_ */
