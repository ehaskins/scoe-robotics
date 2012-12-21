/*
Copyright (c) 2009 Chris B Stones (welcometochrisworld.com)

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include "osmc.h"
#include "utils.h"

//OSMC::osmc(int aHI,int bHI,int aLI,int bLI, int dis) {
	//	this->aHI = aHI;
	//	this->bHI = bHI;
	//	this->aLI = aLI;
	//	this->aLI = bLI;
	//	this->dis = dis;
//}

void Osmc::init(int aLI,int bLI, int dis) {
	pinMode(aLI, OUTPUT);
	pinMode(bLI, OUTPUT);
	pinMode(dis, OUTPUT);
	this->aLI = aLI;
	this->bLI = bLI;
	this->dis = dis;
}

//OSMC::~OSMC() { /* nothing */}
void Osmc::drive(float drive){	
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
	
void Osmc::setEnabled(bool val){
	isEnabled = val;
	digitalWrite( this->dis, val ? LOW : HIGH);
}

bool Osmc::getEnabled(){
	return isEnabled;
}