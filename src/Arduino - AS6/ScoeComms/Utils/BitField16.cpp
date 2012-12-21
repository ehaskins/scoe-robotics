/*
 * BitField16.cpp
 *
 *  Created on: Feb 15, 2011
 *      Author: EHaskins
 */

#include "BitField16.h"

unsigned short BitField16::getBitMask(int index) {
	return 1 << index;
}

bool BitField16::getBit(int index) {
	unsigned short mask = getBitMask(index);
	return (data & mask) == mask;
}
void BitField16::setBit(int index, bool value) {
	unsigned short mask = getBitMask(index);

	bool current = getBit(index);
	if (current != value) {
		if (value) {
			data +=  mask;
		} else {
			data -= mask;
		}
	}
}
