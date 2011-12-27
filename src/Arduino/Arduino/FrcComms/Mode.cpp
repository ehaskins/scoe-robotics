/*
 * Mode.cpp
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#include "Mode.h"

	bool Mode::getFpgaChecksum(){
		return data.getBit(0);
	}
	void Mode::setFpgaChecksum(bool val){
		data.setBit(0, val);
	}

	bool Mode::getCRioChecksum(){
		return data.getBit(1);
	}
	void Mode::setCRioChecksum(bool val){
		data.setBit(1, val);
	}

	bool Mode::getResync(){
		return data.getBit(2);
	}
	void Mode::setResync(bool val){
		data.setBit(2, val);
	}

	bool Mode::getAutonomous(){
		return data.getBit(4);
	}
	void Mode::setAutonomous(bool val){
		data.setBit(4, val);
	}

	bool Mode::getEnabled(){
		return data.getBit(5);
	}
	void Mode::setEnabled(bool val){
		data.setBit(5, val);
	}

	bool Mode::getEStop(){
		return !data.getBit(6);
	}
	void Mode::setEStop(bool val){
		data.setBit(6, !val);
	}

	bool Mode::getReset(){
		return data.getBit(7);
	}
	void Mode::setReset(bool val){
		data.setBit(7, val);
	}
