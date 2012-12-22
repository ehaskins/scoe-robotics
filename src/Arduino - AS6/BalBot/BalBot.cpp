#include <Arduino.h>
#include <Comm\Serial\ScoeComms.h>
#include <Servo\Servo.h>
#include <Encoder\Encoder.h>
#include <RobotModel\RobotModel.h>
#include <RobotModel\RslModelSection.h>
#include <Comm\Udp\UdpComms.h>
#include <Utils\utils.h>
#include <Control\PID.h>
#include <Drivers\Input\AnalogGyro.h>
#include <Drivers\Input\AnalogAccelerometer.h>
#include <Control\SimpleAngleThing.h>
#include "BalanceSection.h"
#include "BalBot.h"


#define BALANCE_SAFTEY_TILT 6

#define LEFT_MOTOR 3
#define RIGHT_MOTOR 2
#define RIGHT_INVERT 1
#define LEFT_INVERT -1


AnalogGyro TiltGyro(9, 1);
AnalogAccelerometer UpAccel(12, 500, true);
AnalogAccelerometer ForwardAccel(13, 500, true);
SimpleAngleThing AngleCalc(&TiltGyro, &ForwardAccel, &UpAccel, 0.98, true);

Encoder leftEnc (21, 18);
Encoder rightEnc (20,17);

Servo left;
Servo right;

#define PULSE_RANGE 750
#define LEFT_CENTER 1500
#define RIGHT_CENTER 1490
#define PLUSE_DEADBAND 80

//PID BalancePID(-40, -2, -25);
PID BalancePID(0, 0, 0);

//SerialInterface beagleComm;
UdpComms udpComm;
TuningDataSection tuningData;
RslModelSection rsl;

void setup() {
	
	pinMode(13, OUTPUT);
	writeLed(true);
	Serial.begin(115200);
	
	Serial.println("Starting up...");
	
	udpComm.isDhcp = true;
	udpComm.receivePort = 8888;
	byte mac[6] = {0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF};
	for (int i = 0; i< 6; i++){
		udpComm.macAddress[i] = mac[i];
	}
	udpComm.init();
	udpComm.robotModel.addSection(&tuningData);
	udpComm.robotModel.addSection(&rsl);
	
	tuningData.p = -0.08f;
	tuningData.i = -0.005f;
	tuningData.d = 0.0005f;
	tuningData.desiredAngle = -8;
	tuningData.spin = 0;
	tuningData.safteyLimit = 3;
	
	// print your local IP address:
	Serial.print("My IP address: ");
	for (byte i = 0; i < 4; i++) {
		// print the value of each byte of the IP address:
		Serial.print(Ethernet.localIP()[i], DEC);
		Serial.print(".");
	}
	Serial.println();
	
	calibrate(2000, 200);
	
	right.attach(RIGHT_MOTOR);
	left.attach(LEFT_MOTOR);

	Serial.println("Ready.");
	writeLed(false);
}

unsigned long lastLoop = 0;
float elapsedSeconds = 0.0;
void loop() {
	long nowMicros = micros();
	if (lastLoop == 0)
	lastLoop = nowMicros;
	elapsedSeconds = (float)(nowMicros - lastLoop) / 1000000;
	
	//beagleComm.poll();
	udpComm.poll();
	
	Serial.print(tuningData.spin);
	if (elapsedSeconds >= 0.01){
		BalancePID.P = tuningData.p;
		BalancePID.I = tuningData.i;
		BalancePID.D = tuningData.d;
		
		/*BalancePID.P = -0.08f;
		BalancePID.I = -0.005f;
		BalancePID.D = 0.0005f;
		*/
		
		lastLoop = nowMicros;
		
		balance(tuningData.desiredAngle, tuningData.spin);
		tuningData.currentAngle = AngleCalc.angle;
		//printAngle();
		//printImuCsv();
		//testCenter();
	}
}

void printAngle(){
	Serial.print("Angle:");
	Serial.print(AngleCalc.angle);
	Serial.print(" Accel X:");
	Serial.print(ForwardAccel.getAcceleration());
	Serial.print(" Accel Y:");
	Serial.print(UpAccel.getAcceleration());
	Serial.print(" Rate:");
	Serial.println(TiltGyro.getRate());
}
int count = 0;
void printImuCsv(){
	int anas[6];
	for (int i = 0; i < 6; i++){
		anas[i] = analogRead(i+8);
	}
	
	Serial.print(count);
	Serial.print(",");
	for (int i = 0; i < 6; i++){
		Serial.print(anas[i]);
		Serial.print(",");
	}
	Serial.print((long)millis());
	
	Serial.println();
	//delay(1);
}

void writeLed(bool state){
	digitalWrite(13, state ? HIGH : LOW);
}

void balance(float desiredAngle, float spin){
	AngleCalc.update();
	float output = 0.0;
	float error = desiredAngle - AngleCalc.angle;
	
	if (abs(error) > tuningData.safteyLimit){
		output = 0;
		BalancePID.iTotal = 0.0;
		spin = 0;
	}
	else{
		output = BalancePID.update(AngleCalc.angle, desiredAngle, AngleCalc.gyro->getRate());
	}
	setDrive(output + spin, output - spin);
}

void setDrive(float leftVal, float rightVal){
	leftVal *= LEFT_INVERT;
	rightVal *= RIGHT_INVERT;
	leftVal = limit(leftVal, -1.0, 1.0);
	rightVal = limit(rightVal, -1.0, 1.0);
	
	leftVal *= PULSE_RANGE;
	rightVal *= PULSE_RANGE;
	
	left.writeMicroseconds(LEFT_CENTER + removeDeadband(leftVal, PLUSE_DEADBAND, PULSE_RANGE));
	right.writeMicroseconds(RIGHT_CENTER + removeDeadband(rightVal, PLUSE_DEADBAND, PULSE_RANGE));
}

void calibrate(int calibrationDelay, int calibrationLoops) {
	Serial.println("Waiting to calibrate...");
	delay(calibrationDelay);
	Serial.println("Calibrating...");

	for (int i = 0; i < calibrationLoops; i++) {
		TiltGyro.calibrate();
		delay(5);
	}
	
	TiltGyro.endCalibrate();

	Serial.println("Calibration complete!");
	Serial.print("GyroCenter:");
	Serial.print(TiltGyro.getCenterValue());
}