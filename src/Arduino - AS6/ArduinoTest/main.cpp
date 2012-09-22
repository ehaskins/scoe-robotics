#include <Arduino.h>

int main(void)
{
	init();

#if defined(USBCON)
	USBDevice.attach();
#endif
	
	setup();
    
	for (;;) {
		loop();
		if (serialEventRun) serialEventRun();
	}
        
	return 0;
}

void setup(){
	Serial.begin(9600);
	
	
	Serial.println("Ready!");
}

void loop(){
	long now = millis();
	
	if (now % 1000 < 500){
		
	}
}
