#include <iostream>

using namespace std;

class Base {
public:
	int a;
public:
	Base() : a(1) {
		printf("\nКонструктор по умолчанию Base");
	}
	Base(Base* obj) : a(obj->a) {
		printf("\nКонструктор с указателем на Base");
	}
	Base(const Base& obj) : a(obj.a) {
		printf("\nКонструктор копирования Base");
	}
	
	virtual ~Base() {
		printf("\nДеструктор Base");
	}
	
};
class Desc : public Base {
public:
	Desc() {
		printf("\nКонструктор по умолчанию Desc");
	}
	Desc(Desc* obj) {
		printf("\nКонструктор с указателем на Desc");
	}
	Desc(const Desc& obj) {
		printf("\nКонструктор копирования Desc");
	}
	~Desc() {
		printf("\nДеструктор Desc");
	}
};

void func1(Base obj) { // Копируется оригинал, изменяется копия (оригинал не изменяется), копия удаляется

};
void func2(Base* obj) { // Работаем с оригинальным объектом и можем его изменить
	Desc* newdesc = dynamic_cast<Desc*>(obj);
	if (newdesc) {
		printf("\nОбъект класса Desc");
	}
	else {
		printf("\nОбъект другого класса");
	}
};
void func3(Base& obj) { // Работаем с оригинальным объектом и можем его изменить
	
};


Base rfunc1() { // Один из самых адекватных способов 
	Base base;
	return base;
}
Base* rfunc2() { // UB (Возвращается указатель на локальный объект, который уже уничтожен)
	Base base;
	return &base;
}
Base& rfunc3() { // UB (Возвращается ссылка на уничтоженный объект)
	Base base;
	return base;
}

Base rfunc4() { // Утечка памяти, не удаляется динамический объект
	Base* base = new Base();
	return  *base; //Именно здесь происходит утечка памяти, так как теряется указатель
}
Base* rfunc5() { // Возвращается адрес объекта, потенциальная утечка памяти если забыть удалить
	Base* base = new Base();
	return base;
}
Base& rfunc6() { // Потанцевальная утечка памяти, можно удалить использовав извращение в виде delete &base
	Base* base = new Base();
	return *base;
}


void func4(shared_ptr<Base> p)
{
	printf("\nshared_ptr transfer, count = %d", p.use_count());
}
void func5(unique_ptr<Base> p)
{
	printf("\nunique_ptr transfer");
}


unique_ptr<Base> rfunc7() {
	unique_ptr <Base> p = make_unique<Base>();
	printf("\nunique_ptr return");
	return p; // Возврат через move-семантику
}
shared_ptr<Base> rfunc8() {
	shared_ptr<Base> p = make_shared<Base>();
	printf("\nshared_ptr return");
	return p;
}



int main()
{
	setlocale(LC_ALL, "RU");

	printf("\n		Передача объектов");
	printf("\n\n   Конструктор Base: ");
	Base base;
	printf("\n   Функция func1: ");
	func1(base);
	printf("\n\n");
	printf("\n   Функция func2: ");
	func2(&base);
	printf("\n\n");
	printf("\n   Функция func3: ");
	func3(base);

	printf("\n\n   Конструктор Desc: ");
	Desc desc;
	printf("\n   Функция func1: ");
	func1(desc);
	printf("\n\n");
	printf("\n   Функция func2: ");
	func2(&desc);
	printf("\n\n");
	printf("\n   Функция func3: ");
	func3(desc);

	printf("\n\n		Создание и возвращение статических объектов");
	printf("\n rfunc1:\n");
	Base base1 = rfunc1(); // Деструктор не вызывается так как копия объекта уходит из функции наружу (в main), а оригинальный base уничтожается
	printf("\na of base1 = %d", base1.a);
	printf("\n\n");
	printf("\n rfunc2:\n");
	Base* base2 = rfunc2(); // Конструктор копирования не вызывается потому что возвращается указатель, а не ссылка
	printf("\na of base2 = %d", base2->a);
	printf("\n\n");
	printf("\n rfunc3:\n");
	Base base3 = rfunc3(); // Вызывается конструктор копирования, но копируется мусор
	printf("\na of base3 = %d", base3.a);
	printf("\n\n");
	printf("\na of base1 = %d", base1.a);
	printf("\na of base2 = %d", base2->a);
	printf("\na of base3 = %d", base3.a);

	printf("\n\n		Создание и возвращение динамических объектов");
	printf("\n rfunc4:\n");
	Base base4 = rfunc4(); //Вовзращаем сам объект, а не указатель, поэтому вызывается конструктор копирования. Копия создаётся в base4
	printf("\na of base4 = %d", base4.a);
	printf("\n\n");
	printf("\n rfunc5:\n");
	Base* base5 = rfunc5();
	printf("\na of base5 = %d", base5->a);
	printf("\n\n");
	printf("\n rfunc6:\n");
	Base& base6 = rfunc6();
	printf("\na of base6 = %d", base6.a);
	printf("\n\n");
	printf("\na of base4 = %d", base4.a);
	printf("\na of base5 = %d", base5->a);
	printf("\na of base6 = %d", base6.a);
	//delete base4; //Нелья, так как base4 - объект на стеке
	delete base5;
	delete &base6;


	printf("\n\n		Использование умных указателей \n");
	printf("\n		unique_ptr \n");
	unique_ptr<Base> uniqueBase1 = make_unique<Base>();
	//unique_ptr<Base> uniqueBase2 = uniqueBase1;
	printf("\n\na of uniqueBase1 = %d", uniqueBase1->a);
	unique_ptr<Base> uniqueBase2 = move(uniqueBase1);
	printf("\nПосле перемещения:");
	printf("\nuniqueBase2: a = %d", uniqueBase2->a);
	if (uniqueBase1) 
		printf("\n uniqueBase1 существует");
	else 
		printf("\nuniqueBase1 пуст\n");


	printf("\n		shared_ptr \n");
	shared_ptr<Base> sharedBase1 = make_shared<Base>();
	printf("\sharedBase1: a = %d (использований: %d)", sharedBase1->a, sharedBase1.use_count());
	shared_ptr<Base> sharedBase2 = sharedBase1;
	printf("\n    После копирования:");
	printf("\nsharedBase1 использований: %d", sharedBase1.use_count());
	printf("\nsharedBase2 использований: %d", sharedBase2.use_count());

	{
		shared_ptr<Base> sharedBase3 = sharedBase1;
		printf("\n    Внутри блока: использований = %d", sharedBase1.use_count());
	}
	
	printf("\n    После блока: использований = %d", sharedBase1.use_count());

	unique_ptr<Base> p1 = make_unique<Base>();
	func5(move(p1)); // Передача в func5, p1 становится пустым
	printf("\n\n");
	unique_ptr<Base> p2 = rfunc7(); // Возврат, p2 теперь владеет объектом, p — опустошён.
	printf("\n\n");
	
	shared_ptr<Base> p3 = make_shared<Base>();
	//func4(p3);
	func4(move(p3));
	printf("\n\n");
	shared_ptr<Base> p4 = rfunc8();
	
	printf("\n\n   Деструкторы: ");
}


