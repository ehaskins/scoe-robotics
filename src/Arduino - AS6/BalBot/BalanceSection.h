/*
* TuningDataSection.h
*
* Created: 10/6/2012 12:23:59 PM
*  Author: EHaskins
*/


#ifndef TUNINGDATASECTION_H_
#define TUNINGDATASECTION_H_


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
	virtual void update(unsigned char data[], unsigned int offset){
		byte *pBytes = &data[offset];
		float* pVal = reinterpret_cast<float*>(pBytes);
		offset += 4;
		
		byte *iBytes = &data[offset];
		float* iVal = reinterpret_cast<float*>(iBytes);
		offset += 4;
		
		byte *dBytes = &data[offset];
		float* dVal = reinterpret_cast<float*>(dBytes);
		offset += 4;
		
		byte *desiredBytes = &data[offset];
		float* desiredVal = reinterpret_cast<float*>(desiredBytes);
		offset += 4;
		
		p = *pVal;
		i = *iVal;
		d = *dVal;
		desiredAngle = *desiredVal;
	}
	virtual void getStatus(unsigned char data[], unsigned int *offset){
		unsigned int pos = *offset;
		byte* currentBytes = reinterpret_cast<byte*>(&currentAngle);
		for (int i = 0; i < 4; i++){
			data[pos++] = currentBytes[i];
		}
		
		*offset = pos;
	}
};


#endif /* TUNINGDATASECTION_H_ */