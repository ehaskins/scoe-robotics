/*
 * AnalogIODefinition.cpp
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#include "AnalogIODefinition.h"
#include <Arduino.h>
AnalogIODefinition::AnalogIODefinition() {
	sampleCount = 0;
	pin = 0;
	enabled = false;
	sampleInterval = 0;

}

void AnalogIODefinition::commLoop(){
	if (sampleCount == 0)
		update(true);
}
void AnalogIODefinition::update(){
	update(false);
}
void AnalogIODefinition::update(bool force){
	unsigned long now = micros();
	unsigned long elapsed = now - lastSampleTime;
	if ((force || (sampleInterval != 0 && elapsed > sampleInterval)) && sampleCount < MAX_AIO_SAMPLES){
		lastSampleTime = now;
		samples[sampleCount].value = (unsigned int)analogRead(pin);
		samples[sampleCount++].delay = elapsed;
	}
}
