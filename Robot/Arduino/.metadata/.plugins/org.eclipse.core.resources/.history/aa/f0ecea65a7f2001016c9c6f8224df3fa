#include <WProgram.h>
#include "UserCode.h"
#include "UserConstants.h"
#include <FrcComms\FRCCommunication.h>
#include <FrcComms\Configuration.h>
#include <Ethernet.h>
#include <FrcComms\CRC32.h>

//const int interval = 250; //Milliseconds
Configuration *config;
FRCCommunication *comm;
UserRobot robot;

int main() {
	setup();
	while (true) {
		loop();
	}
	//Unreachable code but it's required by
	//the compiler
	return 0;
}

void setup() {
	init();
	Serial.begin(9600);

	Serial.println("Ready.");
}

unsigned long lastLoopTime = 0;
unsigned long nextLoopTime = 0;
unsigned long fixedLoopPeriod = 0;
void loop() {

}
