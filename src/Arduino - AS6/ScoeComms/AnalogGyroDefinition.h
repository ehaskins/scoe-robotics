/*
 * GyroDefinition.h
 *
 *  Created on: Aug 13, 2012
 *      Author: EHaskins
 */

#ifndef GYRODEFINITION_H_
#define GYRODEFINITION_H_

#define MIN_INTERVAL 5
class AnalogGyroDefinition {
private:
	bool calibrate;
	bool lastCalibrate;
	unsigned long lastRead;
	long calSum;
	int calCount;
	int center;

public:
	unsigned char pin;
	long long value;
	long rate;
	void update(bool calibrate);
	void loop();
};

#endif /* GYRODEFINITION_H_ */
