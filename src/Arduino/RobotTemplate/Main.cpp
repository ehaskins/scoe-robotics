#include <WProgram.h>
#include "UserCode.h"
#include "UserConstants.h"
#include <Ethernet.h>
#include <FrcComms\CRC32.h>
#include "ScoeComms\ScoeComms.h"
#include "ScoeComms\RslModelSection.h"
#include "ScoeComms\PwmModelSection.h"

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
	Serial.println("1.");
	RslModelSection * rsl = new RslModelSection();
	PwmModelSection * pwm = new PwmModelSection(1500, 2500);
	beagleComm.init();
	Serial.println("2.");
	beagleComm.robotModel.addSection(rsl);
	beagleComm.robotModel.addSection(pwm);
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
