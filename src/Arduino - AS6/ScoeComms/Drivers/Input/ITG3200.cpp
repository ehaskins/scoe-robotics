/*
* ITG3200.cpp
*
* Created: 12/21/2012 6:16:02 PM
*  Author: EHaskins
*/
#include <Arduino.h>
#include "ITG3200.h"
#include "ITG3200Axis.h"

extern "C"{
	#include ".\I2C\twi.h"
}

ITG3200::ITG3200(){
	startSensor(ITG_ADDR);
}
ITG3200::ITG3200(uint8_t id){
	startSensor(id);
}
bool ITG3200::update(){
	unsigned long now = micros();
	unsigned long elapsed = now - lastUpdateMicros;

	//limit update to 100hz since that's out sensor's update rate
	if (elapsed > 10000) {
		float elapsedSeconds = (float)elapsed / 1000000;
		uint8_t data[8];
		
		for (int i = 0; i < 8; i++)
		{
			char val;
			read(TEMP_OUT_H + i, &val);
			data[i] = val;
		}
		
		int tempRaw,xRaw, yRaw, zRaw;
		tempRaw = (data[0]<<8)|data[1];
		xRaw = (data[2]<<8)|data[3];
		yRaw = (data[4]<<8)|data[5];
		zRaw = (data[6]<<8)|data[7];
		
		x->update(xRaw, elapsedSeconds);
		y->update(yRaw, elapsedSeconds);
		z->update(zRaw, elapsedSeconds);
		temp = tempRaw;
		lastUpdateMicros = micros();
		return true;
	}
	return false;
}

void ITG3200::calibrate(){
	int xSum, ySum, zSum;
	xSum = ySum = zSum = 0.0;
	x->setCenterValue(0);
	y->setCenterValue(0);
	z->setCenterValue(0);
	
	for (int i = 0; i < 100; i++){
		while (!this->update()){
			delay(10);
		};
		xSum += x->getRawValue();
		ySum += y->getRawValue();
		zSum += z->getRawValue();
	}
	
	x->setCenterValue(xSum/100);
	y->setCenterValue(ySum/100);
	z->setCenterValue(zSum/100);
	
	Serial.print(x->getCenterValue());
	Serial.print(", ");
	Serial.print(x->getCenterValue());
	Serial.print(", ");
	Serial.println(x->getCenterValue());
}
void ITG3200::startSensor(uint8_t id){
	this->id = id;
	x = new ITG3200Axis(this);
	y = new ITG3200Axis(this);
	z = new ITG3200Axis(this);
	
	twiInit(80000);			//Init. SCL speed to 50 kHz
	
	//Set internal clock to 1kHz with 42Hz LPF and Full Scale to 3 for proper operation
	write(DLPF_FS, DLPF_FS_SEL_0|DLPF_FS_SEL_1|DLPF_CFG_0);
	
	//Set sample rate divider for 100 Hz operation
	write(SMPLRT_DIV, 9);	//Fsample = Fint / (divider + 1) where Fint is 1kHz
	
	//Setup the interrupt to trigger when new data is ready.
	write(INT_CFG, INT_CFG_RAW_RDY_EN | INT_CFG_ITG_RDY_EN);
	
	//Select X gyro PLL for clock source
	write(PWR_MGM, PWR_MGM_CLK_SEL_0);

	resolution = 0.0696; //1/14.375;
}

//Read a register value from the ADXL345
//pre: register_addr is the register address to read
//	   value is a pointer to an integer
//post: value contains the value of the register that was read
//returns: 1-Success
//		   TWSR-Failure (Check out twi.h for TWSR error codes)
//usage: status = accelerometer.read(DEVID, &value); //value is created as an 'int' in main.cpp
char ITG3200::read(char register_addr, char * value){
	twiReset();
	return twiReceive(id, register_addr, value);
}

//Write a value to a register
//pre: register_addre is the register to write to
//	   value is the value to place in the register
//returns: 1-Success
//		   TWSR- Failure
//usage status=accelerometer.write(register_addr, value);
char ITG3200::write(char register_addr, char value){

	twiReset();
	return twiTransmit(id, register_addr, value);
}
