/*
* ADXL345.cpp
*
* Created: 12/21/2012 6:16:14 PM
*  Author: EHaskins
*/
#include <Arduino.h>
#include <Wire\Wire.h>
#include "ADXL345.h"
#include "ADXL345Axis.h"
#include "..\..\Utils\I2CHelpers.h"

ADXL345::ADXL345(){
	startSensor(ADXL_ADDR);
}
ADXL345::ADXL345 (uint8_t id){
	startSensor(id);
}

void ADXL345::update(){
	unsigned long now = micros();
	unsigned long elapsed = now - lastUpdateMicros;

	//limit update to 100hz
	if (elapsed > 10000) {
		uint8_t data[6];
		
		readI2CBytes(id, DATAX0, data, 0, 6);
		
		int xRaw, yRaw, zRaw;
		xRaw = (data[1]<<8)|data[0];
		yRaw = (data[3]<<8)|data[2];
		zRaw = (data[5]<<8)|data[4];
		
		x->update(xRaw);
		y->update(yRaw);
		z->update(zRaw);
	}

	lastUpdateMicros = micros();
}

void ADXL345::startSensor(uint8_t id){
	this->id = id;
	x = new ADXL345Axis();
	y = new ADXL345Axis();
	z = new ADXL345Axis();

	writeI2CByte(id, POWER_CTL, MEASURE);
	writeI2CByte(id, DATA_FORMAT, RANGE_4G);
}