/*
 * GyroDefinition.cpp
 *
 *  Created on: Aug 13, 2012
 *      Author: EHaskins
 */

#include "AnalogGyroDefinition.h"
#include <Arduino.h>

void AnalogGyroDefinition::update(bool calibrate) {
	if (calibrate && !this->calibrate) {
		calSum = 0;
		calCount = 0;
	}
	this->calibrate = calibrate;
	loop();
}
void AnalogGyroDefinition::loop() {
	unsigned long now = millis();
	unsigned long elapsed = now - lastRead;
	if (elapsed >= MIN_INTERVAL) {
		int value = (int)analogRead(pin);
		if (calibrate) {
			calSum += value;
			calCount++;
		} else if (!calibrate && lastCalibrate) {
			center = calSum / calCount;
			this->value = 0;
		}

		lastCalibrate = calibrate;
		int centered = value - center;
		if (centered > -1 && centered < 1)
			centered = 0;

		int duration = lastRead != 0 ? elapsed : 0;

		this->value += centered * duration;
		//this->value = value-center;
		lastRead = now;
	}
}
