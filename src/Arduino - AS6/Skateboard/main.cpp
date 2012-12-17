/*
 * main.cpp
 *
 * Created: 10/4/2012 12:02:23 AM
 *  Author: EHaskins
 */ 
#include "Arduino.h"
#include "SkateBoard.h"
int main(void)
{
	init();

	/*#if defined(USBCON)
	USBDevice.attach();
	#endif*/
	
	setup();
	
	for (;;) {
		loop();
		/*
		Serial.print(micros());
		
		for (int i = 1; i <= 6; i++)
		{
			Serial.print(",");
			Serial.print(analogRead(i));
		}
		Serial.println();
		*/
		//if (serialEventRun) serialEventRun();
	}
	
	return 0;
}