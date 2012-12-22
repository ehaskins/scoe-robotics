/*
* ITG3200Axis.h
*
* Created: 12/21/2012 9:11:49 PM
*  Author: EHaskins
*/


#ifndef ITG3200AXIS_H_
#define ITG3200AXIS_H_

#include "ITG3200.h"
#include "Gyro.h"

class ITG3200Axis : public Gyro{
	public:
	ITG3200Axis(ITG3200 * parent){
		this->parent = parent;
	}
	void update(){
		parent->update();
	}
	void update(int rawValue);

	private:
	ITG3200 *parent;
	unsigned long lastMicros;
};

#endif /* ITG3200AXIS_H_ */