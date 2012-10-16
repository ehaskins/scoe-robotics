#include <Arduino.h>
#include <ScoeComms.h>
#include <Servo\Servo.h>
#include <Encoder\Encoder.h>
#include <RobotModel.h>
#include <RslModelSection.h>
#include "utils.h"
#include "PID.h"
#include "Gyro.h"
#include "Accelerometer.h"
#include "SimpleAngleThing.h"
#include "BalanceSection.h"
#include "BalBot.h"

#define BALANCE_SAFTEY_TILT 3

#define LEFT_MOTOR 3
#define RIGHT_MOTOR 2
#define RIGHT_INVERT 1
#define LEFT_INVERT -1

Gyro TiltGyro(1, 0.7);
Accelerometer UpAccel(4, 500, true);
Accelerometer ForwardAccel(5, 500, true);
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

SerialInterface beagleComm;
TuningDataSection tuningData;
RslModelSection rsl;

void setup() {
	
	pinMode(13, OUTPUT);
	writeLed(true);
	Serial.begin(115200);
	
	beagleComm.init(&Serial);
	beagleComm.robotModel.addSection(&tuningData);
	beagleComm.robotModel.addSection(&rsl);
	
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
	
	beagleComm.poll();
	if (elapsedSeconds > 0.015){
		delay(2000);
	}
	else if (elapsedSeconds >= 0.01){
		BalancePID.P = tuningData.p;
		BalancePID.I = tuningData.i;
		BalancePID.D = tuningData.d;
		double desiredAngle = tuningData.desiredAngle;
		
		lastLoop = nowMicros;
		
		balance(desiredAngle, 0);
		tuningData.currentAngle = AngleCalc.angle;
		//printSensors();
		//printImuCsv();
		//testCenter();
	}
}

void writeLed(bool state){
	digitalWrite(13, state ? HIGH : LOW);
}

void balance(float desiredAngle, float spin){
	AngleCalc.update();
	float output = 0.0;
	float error = desiredAngle - AngleCalc.angle;
	
	if (abs(error) > BALANCE_SAFTEY_TILT){
		output = 0;
		BalancePID.iTotal = 0.0;
		spin = 0;
	}
	else{
		output = BalancePID.update(AngleCalc.angle, desiredAngle, AngleCalc.gyro->rate);
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
	Serial.print(TiltGyro.center);
}