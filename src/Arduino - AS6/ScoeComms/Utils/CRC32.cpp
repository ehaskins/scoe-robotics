/*
* CRC32.cpp
*
*  Created on: Feb 19, 2011
*      Author: EHaskins
*/

//FROM http://excamera.com/sphinx/article-crc.html

#include "CRC32.h"
#include <Arduino.h>

const PROGMEM prog_uint32_t crc_table[16] = {
	0x00000000, 0x1db71064, 0x3b6e20c8, 0x26d930ac,
	0x76dc4190, 0x6b6b51f4, 0x4db26158, 0x5005713c,
	0xedb88320, 0xf00f9344, 0xd6d6a3e8, 0xcb61b38c,
	0x9b64c2b0, 0x86d3d2d4, 0xa00ae278, 0xbdbdf21c
};

unsigned long update_crc(unsigned long crc, byte data)
{
	byte tbl_idx;
	tbl_idx = crc ^ (data >> (0 * 4));
	crc = pgm_read_dword_near(crc_table + (tbl_idx & 0x0f)) ^ (crc >> 4);
	tbl_idx = crc ^ (data >> (1 * 4));
	crc = pgm_read_dword_near(crc_table + (tbl_idx & 0x0f)) ^ (crc >> 4);
	return crc;
}

/* Return the CRC of the bytes buf[0..len-1]. */
unsigned long crc(unsigned char *buf, int len) {
	unsigned long crc = ~0L;
	for (int i = 0; i < len; i++){
		crc = update_crc(crc, buf[i]);
	}
	return ~crc;
}

