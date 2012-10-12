/*
 * CRC32.h
 *
 *  Created on: Feb 19, 2011
 *      Author: EHaskins
 */

#ifndef CRC32_H_
#define CRC32_H_

#include <avr/pgmspace.h>

typedef unsigned long PROGMEM prog_uint32_t;

unsigned long update_crc(unsigned long crc, unsigned char *buf, int len);
unsigned long crc(unsigned char *buf, int len);
#endif /* CRC32_H_ */
