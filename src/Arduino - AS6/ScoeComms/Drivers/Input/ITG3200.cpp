/*
* ITG3200.cpp
*
* Created: 12/21/2012 6:16:02 PM
*  Author: EHaskins
*/
#include <Arduino.h>
#include <Wire\Wire.h>
#include "ITG3200.h"
#include "ITG3200Axis.h"
#include "..\..\Utils\I2CHelpers.h"
ITG3200::ITG3200(){
	startSensor(ITG_ADDR);
}
ITG3200::ITG3200(uint8_t id){
	startSensor(id);
}
void ITG3200::update(){
	unsigned long now = micros();
	unsigned long elapsed = now - lastUpdateMicros;

	//limit update to 100hz since that's out sensor's update rate
	if (elapsed > 10000) {
		uint8_t data[8];
		
		readI2CBytes(id, TEMP_OUT_H, data, 0, 8);
		
		int tempRaw,xRaw, yRaw, zRaw;
		tempRaw = (data[0]<<8)|data[1];
		xRaw = (data[2]<<8)|data[3];
		yRaw = (data[4]<<8)|data[5];
		zRaw = (data[6]<<8)|data[7];
		
		x->update(xRaw);
		y->update(yRaw);
		z->update(zRaw);
	}

	lastUpdateMicros = micros();
}
void ITG3200::startSensor(uint8_t id){
	this->id = id;
	x = new ITG3200Axis(this);
	y = new ITG3200Axis(this);
	z = new ITG3200Axis(this);

	//Set internal clock to 1kHz with 42Hz LPF and Full Scale to 3 for proper operation
	writeI2CByte(id, DLPF_FS, DLPF_FS_SEL_0|DLPF_FS_SEL_1|DLPF_CFG_0);
	
	//Set sample rate divider for 100 Hz operation
	writeI2CByte(id, SMPLRT_DIV, 9);	//Fsample = Fint / (divider + 1) where Fint is 1kHz
	
	//Setup the interrupt to trigger when new data is ready.
	writeI2CByte(id, INT_CFG, INT_CFG_RAW_RDY_EN | INT_CFG_ITG_RDY_EN);
	
	//Select X gyro PLL for clock source
	writeI2CByte(id, PWR_MGM, PWR_MGM_CLK_SEL_0);

	resolution = 1/14.375;
}