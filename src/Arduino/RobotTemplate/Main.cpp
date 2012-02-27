#include <Arduino.h>
#include <ScoeComms\CRC32.h>
#include <ScoeComms\ScoeComms.h>
#include <ScoeComms\RslModelSection.h>
#include <ScoeComms\PwmModelSection.h>
#include <ScoeComms\AnalogIOSection.h>
#include <ScoeComms\DioSection.h>
#include <ScoeComms\DutyCycleModelSection.h>
#include <ScoeComms\EncoderModelSection.h>

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

	uint8_t testData[45];
	testData[0] = 3;
	testData[1] = 179;
	testData[4] = 17;
	testData[5] = 1;
	testData[8] = 3;
	testData[9] = 4;
	testData[10] = 7;
	testData[11] = 1;
	testData[13] = 100;
	testData[14] = 1;
	testData[15] = 2;
	testData[18] = 4;
	testData[21] = 2;
	testData[22] = 21;
	testData[24] = 5;
	testData[30] = 1;
	testData[40] = 3;

	uint32_t testCrc = crc(testData, 45);

	Serial.print("Calculated:");
	Serial.println(testCrc);
	Serial.print("Old sent:");
	Serial.println(631359595);
	Serial.print("correct:");
	Serial.println(4086425666);

	/*RslModelSection * rsl = new RslModelSection();
	PwmModelSection * pwm = new PwmModelSection(500, 2500);
	AnalogIOSection *analog = new AnalogIOSection();
	DutyCycleModelSection * dutyCycle = new DutyCycleModelSection();
	DioSection *dio = new DioSection();
	EncoderModelSection *enc = new EncoderModelSection();

	beagleComm.init(&Serial);
	beagleComm.robotModel.addSection(rsl);
	beagleComm.robotModel.addSection(pwm);
	beagleComm.robotModel.addSection(analog);
	beagleComm.robotModel.addSection(dio);
	beagleComm.robotModel.addSection(dutyCycle);
	beagleComm.robotModel.addSection(enc);*/

	Serial.println("Ready.");
}

unsigned long lastLoopTime = 0;
unsigned long nextLoopTime = 0;
unsigned long fixedLoopPeriod = 0;
void loop() {
	//beagleComm.poll();
}
