/*
 * main.cpp
 *
 * Created: 10/4/2012 12:02:23 AM
 *  Author: EHaskins
 */ 
#include "Arduino.h"
#include "Balbot.h"
int main(void)
{
	init();

	/*#if defined(USBCON)
	USBDevice.attach();
	#endif*/
	
	setup();
	
	for (;;) {
		loop();
		//if (serialEventRun) serialEventRun();
	}
	
	return 0;
}