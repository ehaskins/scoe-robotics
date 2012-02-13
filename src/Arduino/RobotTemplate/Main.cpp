#include <WProgram.h>
#include "UserCode.h"
#include "UserConstants.h"
#include <Ethernet.h>
#include <FrcComms\CRC32.h>
#include "ScoeComms\ScoeComms.h"
#include "ScoeComms\RslModelSection.h"
#include "ScoeComms\PwmModelSection.h"
#include "ScoeComms\AnalogIOSection.h"
#include "ScoeComms\DioSection.h"

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
	RslModelSection * rsl = new RslModelSection();
	PwmModelSection * pwm = new PwmModelSection(500, 2500);
	AnalogIOSection *analog = new AnalogIOSection();
	DioSection *dio = new DioSection();
	beagleComm.init();
	beagleComm.robotModel.addSection(rsl);
	beagleComm.robotModel.addSection(pwm);
	beagleComm.robotModel.addSection(analog);
	beagleComm.robotModel.addSection(dio);
	//beagleComm->commSerial = &Serial;
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
