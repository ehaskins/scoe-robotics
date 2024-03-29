/*
	Example of APM_BMP085 (absolute pressure sensor) library.
	Code by Jordi MuÒoz and Jose Julio. DIYDrones.com
*/

#include <Wire.h>
#include <APM_BMP085.h> // ArduPilot Mega BMP085 Library

APM_BMP085_Class APM_BMP085;

unsigned long timer;

void setup()
{	
	Serial.begin(115200);
	Serial.println("ArduPilot Mega BMP085 library test");
	Serial.println("Initialising barometer..."); delay(100);
	APM_BMP085.Init();	 // APM ADC initialization
	Serial.println("initialisation complete."); delay(100);
	delay(1000);
	timer = millis();
}

void loop()
{
	int ch;
	float tmp_float;
	float Altitude;
	
	if((millis()- timer) > 50){
		timer = millis();
		APM_BMP085.Read();
		Serial.print("Pressure:");
		Serial.print(APM_BMP085.Press);
		Serial.print(" Temperature:");
		Serial.print(APM_BMP085.Temp / 10.0);
		Serial.print(" Altitude:");
		tmp_float = (APM_BMP085.Press / 101325.0);
		tmp_float = pow(tmp_float, 0.190295);
		Altitude = 44330 * (1.0 - tmp_float);
		Serial.print(Altitude);
		Serial.println();
	}
}
