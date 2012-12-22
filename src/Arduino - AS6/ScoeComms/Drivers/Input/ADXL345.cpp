/*
* ADXL345.cpp
*
* Created: 12/21/2012 6:16:14 PM
*  Author: EHaskins
*/
#include <Arduino.h>
#include "ADXL345.h"
#include "ADXL345Axis.h"
extern "C"{
	#include ".\I2C\twi.h"
}

ADXL345::ADXL345(){
	startSensor(ADXL_ADDR);
}
ADXL345::ADXL345 (uint8_t id){
	startSensor(id);
}

bool ADXL345::update(){
	unsigned long now = micros();
	unsigned long elapsed = now - lastUpdateMicros;
	
	//limit update to 100hz
	if (elapsed > 10000) {
	uint8_t data[6] = {0, 0, 0, 0, 0, 0};
	
	//readI2CBytes(id, DATAX0, data, 0, 6);
	
	for (int i = 0; i < 6; i++)
	{
		char val;
		read(DATAX0 + i, &val);
		data[i] = val;
	}
	
	int xRaw, yRaw, zRaw;
	xRaw = (data[1]<<8)|data[0];
	yRaw = (data[3]<<8)|data[2];
	zRaw = (data[5]<<8)|data[4];
	
	x->update(xRaw);
	y->update(yRaw);
	z->update(zRaw);
	
	lastUpdateMicros = micros();
	return true;
}
return false;

}

//Read a register value from the ADXL345
//pre: register_addr is the register address to read
//	   value is a pointer to an integer
//post: value contains the value of the register that was read
//returns: 1-Success
//		   TWSR-Failure (Check out twi.h for TWSR error codes)
//usage: status = accelerometer.read(DEVID, &value); //value is created as an 'int' in main.cpp
char ADXL345::read(char register_addr, char * value){
	twiReset();
	return twiReceive(id, register_addr, value);
}

//Write a value to a register
//pre: register_addre is the register to write to
//	   value is the value to place in the register
//returns: 1-Success
//		   TWSR- Failure
//usage status=accelerometer.write(register_addr, value);
char ADXL345::write(char register_addr, char value){

	twiReset();
	return twiTransmit(id, register_addr, value);
}

void ADXL345::startSensor(uint8_t id){
	twiInit(50000);
	this->id = id;
	lastUpdateMicros = 0;
	x = new ADXL345Axis();
	y = new ADXL345Axis();
	z = new ADXL345Axis();

	write(POWER_CTL, MEASURE);
	write(DATA_FORMAT, RANGE_4G);
}