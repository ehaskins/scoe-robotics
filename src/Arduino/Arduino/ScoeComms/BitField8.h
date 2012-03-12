/*
 * BitField8.h
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#ifndef BITFIELD8_H_
#define BITFIELD8_H_

struct BitField8{
private:
	unsigned char getBitMask(int);
public:
	unsigned char data;

	bool getBit(int);
	void setBit(int, bool);
};


#endif /* BITFIELD8_H_ */
