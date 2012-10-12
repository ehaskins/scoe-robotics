/*
 * CRC32.h
 *
 *  Created on: Feb 19, 2011
 *      Author: EHaskins
 */

#ifndef CRC32_H_
#define CRC32_H_

void make_crc_table(void);
unsigned long update_crc(unsigned long crc, unsigned char *buf, int len);
unsigned long crc(unsigned char *buf, int len);
#endif /* CRC32_H_ */
