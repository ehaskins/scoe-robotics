/*
 * BitField16.h
 *
 *  Created on: Feb 15, 2011
 *      Author: EHaskins
 */

#ifndef BITFIELD16_H_
#define BITFIELD16_H_

struct BitField16{
private:
	unsigned short getBitMask(int);
public:
	unsigned short data;

	bool getBit(int);
	void setBit(int, bool);
};

#endif /* BITFIELD16_H_ */
