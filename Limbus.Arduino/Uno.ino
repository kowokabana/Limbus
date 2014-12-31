int motorPin = 9;  // an diesem Pin hängt der Motor

void setup() {
	Serial.begin(9600);
	pinMode(2, OUTPUT); // DigOut2
	pinMode(3, OUTPUT); // ~PWM
	pinMode(4, OUTPUT); // DigOut4
	pinMode(5, OUTPUT); // ~PWM
	pinMode(6, OUTPUT); // ~PWM
	pinMode(7, OUTPUT); // DigOut7
	pinMode(8, INPUT); // DigIn8
	pinMode(9, OUTPUT); // //~PWM
	pinMode(10, OUTPUT); // ~PWM
	pinMode(11, OUTPUT); // ~PWM
	pinMode(12, INPUT); //DigIn12
	pinMode(13, INPUT); //DigIn13
}

void printAnalogs()
{
  	Serial.print("A0: ");
  	Serial.println(analogRead(A0));
  	/*Serial.print("A1: ");
  	Serial.println(analogRead(A1));
	Serial.print("A2: ");
	Serial.println(analogRead(A2));
	Serial.print("A3: ");
	Serial.println(analogRead(A3));
	Serial.print("A4: ");
	Serial.println(analogRead(A4));
	Serial.print("A5: ");
	Serial.println(analogRead(A5));*/
}

void serialToAnalogWrite()
{
	int i=0;
	int pin = -1;
	int val = -1;
	char valStr[4] = {};

	if (Serial.available() > 0) {
		memset(valStr, 0, sizeof(valStr));
		while (Serial.available() > 0 && i < sizeof(valStr) - 1 + 2)
		{
			if(i==0) pin = atoi(Serial.read()); // read pin to write
			else if(i==1) continue; // skip separator ":"
			else inblock[i-2] = Serial.read(); // read value

			i++;
			delay(3);
		}
	}

	val = atoi(valStr);

	if (val < 0) val = 0;
	else if (val > 255) val = 255;

	analogWrite(pin, val);
}

void loop() {
	int potVal = analogRead(A0);
	Serial.print("A0:"); Serial.println(potVal);
	Serial.print("A1:"); Serial.println(potVal+50);

	// scale the numbers from the pot 
	//int angle = map(potVal, 0, 1023, 0, 179);
	serialToAnalogWrite();
}
