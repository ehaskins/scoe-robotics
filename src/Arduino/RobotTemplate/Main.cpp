#include <Arduino.h>
#include <ScoeComms\CRC32.h>
#include <ScoeComms\ScoeComms.h>
#include <ScoeComms\RslModelSection.h>
#include <ScoeComms\PwmModelSection.h>
#include <ScoeComms\AnalogIOSection.h>
#include <ScoeComms\DioSection.h>
#include <ScoeComms\DutyCycleModelSection.h>
#include <ScoeComms\EncoderModelSection.h>
#include <ScoeComms\EchoSection.h>

ScoeComms beagleComm;

int main() {
	setup();
	while (true) {
		loop();
	}
	//Unreachable code but it's required by the compiler
	return 0;
}

void setup() {
	init();

	Serial.begin(115200);
	//Serial.begin(1000000);

	RslModelSection * rsl = new RslModelSection();
	PwmModelSection * pwm = new PwmModelSection(500, 2500);
	AnalogIOSection *analog = new AnalogIOSection();
	DutyCycleModelSection * dutyCycle = new DutyCycleModelSection();
	DioSection *dio = new DioSection();
	EncoderModelSection *enc = new EncoderModelSection();
	EchoSection *echo = new EchoSection();

	beagleComm.init(&Serial);
	beagleComm.robotModel.addSection(rsl);
	beagleComm.robotModel.addSection(pwm);
	beagleComm.robotModel.addSection(analog);
	beagleComm.robotModel.addSection(dio);
	beagleComm.robotModel.addSection(dutyCycle);
	beagleComm.robotModel.addSection(enc);
	beagleComm.robotModel.addSection(echo);

	Serial.println("Ready.");
}

unsigned long lastLoopTime = 0;
unsigned long nextLoopTime = 0;
unsigned long fixedLoopPeriod = 0;
void loop() {
	beagleComm.poll();
	/*while (Serial.available()){
		Serial.write(Serial.read());
	}*/
}
