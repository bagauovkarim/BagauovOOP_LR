#include <iostream>
using namespace std;


class Shape {

protected:
    int x;
    int y;


public:
    Shape() : x(1), y(1) {
        printf("\nКонструктор Shape по умолчанию: x = %d, y = %d", x, y);
    }

    Shape(int x, int y) : x(x), y(y) {
        printf("\nКонструктор Shape с параметрами: x = %d, y = %d", x, y);
    }

    Shape(const Shape& shape) : x(shape.x), y(shape.y) {
        printf("\nКонструктор Figure копирования: x = %d, y = %d", x, y);
    }

    virtual ~Shape() {
        printf("\nДеструктор Shape");
    }

    virtual void area() {
        printf("\narea = 0 (Shape)");
    }

    void method1() {
        method2();
        printf("\nМетод 1 для Shape");
    }
    void method2() {
        printf("\nМетод 2 для Shape");
    }

    void method3() {
        printf("\nМетод 3 для Shape");
    }

    virtual string className() {
        return "Shape";
    }
    virtual bool isA(const string& who) {
        return who == "Shape";
    }

};


class Circle : public Shape {

public:
    int radius;
    
public:
    Circle() : radius(10) {
        printf("\nКонструктор Circle по умолчанию: x = %d, y = %d, radius = %d", x, y, radius);
    }

    Circle(int radius) : radius(radius) {
        printf("\nКонструктор Circle с параметрами: x = %d, y = %d, radius = %d", x, y, radius);
    }

    Circle(const Circle& circle) : radius(circle.radius) {
        printf("\nКонструктор Circle копирования: x = %d, y = %d, radius = %d", x, y, radius);
    }

    ~Circle() {
        printf("\nДеструктор Circle");
    }

    void area() override {
        printf("\narea = %.1f (Circle)", 3.14 * radius * radius);
    }

    void method2() {
        printf("\nМетод 2 для Circle");
    }

    void method3() {
        printf("\nМетод 3 для Circle");
    }

    string className() override {
        return "Circle";
    }
    bool isA(const string &who) override {
        return (who == "Circle") || Shape::isA(who);
    }
};



int main()
{
    setlocale(LC_ALL, "RU");

    printf("\n           Перекрываемые методы\n");

    Shape* shape1 = new Shape();
    Circle* circle1 = new Circle();
    Shape* shape2 = new Circle();
    shape1->area();
    circle1->area();


    printf("\n\n         Виртуальный деструктор\n");

    delete shape2;

    printf("\n\n         Методы 1 и 2\n");

    shape1->method1();
    circle1->method1();


    printf("\n\n         Метод 3\n");

    shape1->method3();
    circle1->method3();


    printf("\n\n        Принадлежность методом className");
    printf("\n\nClassname of shape1: %s", shape1->className().c_str());
    printf("\nClassname of circle1: %s", circle1->className().c_str());
    

    printf("\n\n        Принадлежность методом isA");
    printf("\n\nisA Shape (is Shape --> Abracadabra?): %d", shape1->isA("Abracadabra"));
    printf("\nisA Shape (is Shape --> Shape?): %d", shape1->isA("Shape"));
    printf("\nisA Circle (is Circle --> Shape?): %d", circle1->isA("Shape"));
    printf("\nisA Circle (is Circle --> Circle?): %d", circle1->isA("Circle"));

    
    printf("\n\n         Опасное приведение типов\n");
    Shape* shape_unsafecast = new Circle(5); // Указатель типа Shape указывает на объект класса Circle
    Circle* circle_unsafecast = (Circle*)shape_unsafecast; // Безопасно, если указатель shape_unsafecast действительно указывает на объект класса Circle
    printf("\nОпасное приведение: radius = %d", circle_unsafecast->radius);
    delete shape_unsafecast;
    //delete circle_unsafecast; UB
    
    Shape* shape1_unsafecast = new Shape(5, 5); // Указатель типа shape1_unsafecast указывает на объект класса Shape
    Circle* circle1_unsafecast = (Circle*)shape1_unsafecast;
    printf("\nОпасное приведение (UB): radius = %d", circle1_unsafecast->radius); // Выведет мусор, т.к. объект класса Circle не имеет radius
    
    
    printf("\n\n         Опасное приведение с предварительной проверкой типов");
    
    if (circle1_unsafecast->isA("Circle")) {
        printf("\nОпасное приведение (с предварительной проверкой): radius = %d", circle1_unsafecast->radius);
    }
    else {
        printf("\nОпасное приведение (с предварительной проверкой): not Circle");
    }
    delete shape1_unsafecast;
    //delete circle1_unsafecast; UB
    
    printf("\n\n         Использование стандартных средств языка (dynamic_cast)");
    
    Shape* shape1_safecast = new Shape();
    Shape* shape2_safecast = new Circle();
    Circle* circle1_safecast = dynamic_cast <Circle*>(shape1_safecast);
    printf("\n    Shape --> Circle ?");
    if (circle1_safecast) {
        printf("\n    Приведение с помощью dynamic_cast: radius = %d", circle1_safecast->radius);
    }
    else {
        printf("\n    not a Circle");
    }
    
    printf("\n    Circle -> Circle ?");
    Circle* circle2_safecast = dynamic_cast <Circle*>(shape2_safecast);
    if (circle2_safecast) {
        printf("\n    Приведение с помощью dynamic_cast: radius = %d", circle2_safecast->radius);
    }
    else {
        printf("\n    not a Circle");
    }

    printf("\n\n");
    
    delete shape1;
    delete circle1;
    delete shape1_safecast;
    delete shape2_safecast;
    //delete circle1_safecast; UB
    //delete circle2_safecast; UB
}
    
