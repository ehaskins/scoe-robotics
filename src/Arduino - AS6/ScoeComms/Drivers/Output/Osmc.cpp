#include <Arduino.h>
#include "Osmc.h"
#include "..\..\Utils\utils.h"

Osmc::Osmc(int aLI,int bLI, int dis) {
	pinMode(aLI, OUTPUT);
	pinMode(bLI, OUTPUT);
	pinMode(dis, OUTPUT);
	this->aLI = aLI;
	this->aLI = bLI;
	this->dis = dis;
	setIsEnabled(false);
	setOutput(0.0);
}

void Osmc::updateOutput(){	
	float drive = output;
	
	if (drive < -1)
	drive = -1;
	else if (drive > 1)
	drive = 1;
	
	unsigned char power = (drive >= 0 ? drive  : -drive) * 255;
	if (drive > 0){
		analogWrite(this->aLI, power);
		digitalWrite(this->bLI, LOW);
		Serial.print("Forward:");
		Serial.println(power);
	}
	else if (drive < 0){
		digitalWrite(this->aLI, LOW);
		analogWrite(this->bLI, power);
		
		Serial.print("Reverse:");
		Serial.println(power);
	}
	else{
		digitalWrite(this->aLI, LOW);
		digitalWrite(this->bLI, LOW);
		
		Serial.print("Brake:");
	}
}
	
void Osmc::updateEnabled(){
	digitalWrite( this->dis, isEnabled ? LOW : HIGH);
}