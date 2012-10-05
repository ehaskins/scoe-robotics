#include <Arduino.h>
#include <CRC32.h>
#include <ScoeComms.h>
#include <RslModelSection.h>
#include <PwmModelSection.h>
#include <AnalogIOSection.h>
#include <DioSection.h>
#include <DutyCycleModelSection.h>
#include <EncoderModelSection.h>
#include <AnalogGyroDefinition.h>

ScoeComms beagleComm;

AnalogGyroDefinition * gyroDef;
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

void setup() {
	init();

	Serial.begin(115200);
	//Serial.begin(1000000);

	//gyroDef = new AnalogGyroDefinition();
	//gyroDef->pin = 0;

	// Controls the robot status light.
	RslModelSection * rsl = new RslModelSection();

	//// Controls PWM signal generation, for the speed controllers.
	//PwmModelSection * pwm = new PwmModelSection(500, 2500);
	//
	//// Controls analog I/O.
	//AnalogIOSection * analog = new AnalogIOSection();
	//
	//// Controls duty-cycle computation.
	//DutyCycleModelSection * dutyCycle = new DutyCycleModelSection();
	//
	//// Controls digital I/O.
	//DioSection *dio = new DioSection();
	//
	//// Controls data acquisition from the quadrature shaft encoders.
	//EncoderModelSection *enc = new EncoderModelSection();

	beagleComm.init(&Serial);
	beagleComm.robotModel.addSection(rsl);
	//beagleComm.robotModel.addSection(pwm);
	//beagleComm.robotModel.addSection(analog);
	//beagleComm.robotModel.addSection(dio);
	//beagleComm.robotModel.addSection(dutyCycle);
	//beagleComm.robotModel.addSection(enc);

	Serial.println("Ready.");
}

unsigned long startMillis = 0;

void loop() {
	beagleComm.poll();

	unsigned long now = millis();
	if (startMillis == 0)
	startMillis = now;

	bool cal = (now - startMillis < 1000);
	gyroDef->update(cal);
	Serial.println((long) gyroDef->value);
}
