#ifndef OSMC_H
#define OSMC_H

#include "Motor.h"

class Osmc : public Motor
{
	public:
	int aLI,bLI,dis;

	Osmc (int ,int ,int );
	//~OSMC(); // deconstrutor?
	
	protected:
	void updateOutput();
	void updateEnabled();
};

#endif
