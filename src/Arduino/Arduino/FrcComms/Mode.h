/*
 * Mode.h
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#ifndef MODE_H_
#define MODE_H_

#include "BitField8.h"

struct Mode {
public:
	BitField8 data;

	bool getEnabled();
	void setEnabled(bool);
	bool getResync();
	void setResync(bool);
	bool getEStop();
	void setEStop(bool);
	bool getAutonomous();
	void setAutonomous(bool);
	bool getFpgaChecksum();
	void setFpgaChecksum(bool);
	bool getCRioChecksum();
	void setCRioChecksum(bool);
	bool getReset();
	void setReset(bool);
};

#endif /* MODE_H_ */
