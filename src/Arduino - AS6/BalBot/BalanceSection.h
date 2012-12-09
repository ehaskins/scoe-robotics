/*
* TuningDataSection.h
*
* Created: 10/6/2012 12:23:59 PM
*  Author: EHaskins
*/


#ifndef TUNINGDATASECTION_H_
#define TUNINGDATASECTION_H_

#include <ByteReader.h>
#include <RobotModelSection.h>

class TuningDataSection : public RobotModelSection{
	public:
	TuningDataSection(){
		this->sectionId = 255;
	}
	float p;
	float i;
	float d;
	float currentAngle;
	float desiredAngle;
	float spin;
	float safteyLimit;
	virtual void update(unsigned char data[], unsigned int offset){
	
		p = readFloat(data, &offset);
		i = readFloat(data, &offset);
		d = readFloat(data, &offset);
		safteyLimit = readFloat(data, &offset);
		desiredAngle = readFloat(data, &offset);
		spin = readFloat(data, &offset);
		
	}
	virtual void getStatus(unsigned char data[], unsigned int *offset){
		byte* currentBytes = reinterpret_cast<byte*>(&currentAngle);
		for (int i = 0; i < 4; i++){
			data[(*offset)++] = currentBytes[i];
		}
	}
};


#endif /* TUNINGDATASECTION_H_ */