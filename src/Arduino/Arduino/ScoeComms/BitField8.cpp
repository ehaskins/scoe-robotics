/*
 * BitField8.cpp
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#include "BitField8.h"

unsigned char BitField8::getBitMask(int index) {
	return 1 << index;
}

bool BitField8::getBit(int index) {
	unsigned char mask = getBitMask(index);
	return (data & mask) == mask;
}
void BitField8::setBit(int index, bool value) {
	unsigned char mask = getBitMask(index);

	bool current = getBit(index);
	if (current != value) {
		if (value) {
			data +=  mask;
		} else {
			data -= mask;
		}
	}
}
