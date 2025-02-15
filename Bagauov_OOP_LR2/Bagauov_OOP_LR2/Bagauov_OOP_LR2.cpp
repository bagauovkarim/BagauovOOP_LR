#include <iostream>
#include <string>

using namespace std;


class Animal {

private:
    int height;
    int weight;
    string color;

public:
    Animal() : height(0), weight(0), color("white") {
        printf("Вызван конструктор по умолчанию класса Animal \n");
    }
    Animal(int height, int weight, string color) : height(height), weight(weight), color(color) {
        printf("Вызван конструктор с параметрами класса Animal \n");
    }
    Animal(const Animal& Animal_copy) : height(Animal_copy.height), weight(Animal_copy.weight), color(Animal_copy.color) {
        printf("Вызван конструктор копирования класса Animal \n");
    }
    ~Animal() {
        printf("Вызван деструктор класса Animal \n");
    }

    
    int height_getter() {
        return height;
    }
    int weight_getter() {
        return weight;
    }
    string color_getter() {
        return color;
    }
    void height_setter(int h) {
        if (height > 0) {
            height = h;
        }
        else
            printf("Рост не может быть отрицательным!\n");
    }
    void weight_setter(int w) {
        if (weight > 0) {
            weight = w;
        }
        else
            printf("Вес не может быть отрицательным!\n");
    }
    void color_setter(string c) {
        color = c;
    }

    virtual void sound();
};

void Animal::sound() {
    printf("*silence*...\n");
}

class Bird : public Animal {

private:
    int fly_speed;
    string egg_color;
public:
    Bird() : Animal(), fly_speed(0), egg_color("white") {
        printf("Вызван конструктор по умолчанию класса Bird\n");
    }
    Bird(int height, int weight, string color, int fly_speed, string egg_color) : Animal(height, weight, color), fly_speed(fly_speed), egg_color(egg_color) {
        printf("Вызван конструктор с параметрами класса Bird\n");
    }
    Bird(const Bird& bird_copy) : Animal(bird_copy), fly_speed(bird_copy.fly_speed), egg_color(bird_copy.egg_color) {
        printf("Вызван конструктор копирования класса Bird\n");
    }
    ~Bird() {
        printf("Вызван деструктор класса Bird\n");
    }

    void height_setter(int h) {
        if (height_getter() > 0 and height_getter() < 10) {
            height_setter(h);
        }
        else
            printf("Рост не может быть отрицательным и больше 10!\n");
    }
    void weight_setter(int w) {
        if (weight_getter() > 0 and weight_getter() < 10) {
            weight_setter(w);
        }
        else
            printf("Вес не может быть отрицательным!\n");
    }
    void height_setter(string c) {
        color_setter(c);
    }

    void setter_fly_speed(int f) {
        if (fly_speed > 0) {
            fly_speed = f;
        }
        else
            printf("Скорость птицы не может быть отрицательной! \n");
    }

    void setter_egg_color(string e) {
        egg_color = e;
    }

    int getter_fly_speed() {
        return fly_speed;
    }

    string getter_egg_color() {
        return egg_color;
    }

    void sound() override {
        printf("*bird's sound*...\n");
    }
};




int main() {
    setlocale(LC_ALL, "RU");
    Animal wtf_animal;
    Animal cat(1, 3, "black");
    Animal cat2 = cat;


    Animal* cat_pointer = new Animal;
    Animal* sobaka_pointer = new Animal(1, 4, "green");
    Animal* popugai_pointer = new Animal();

   
    //cat.sound();
    string sobaka_color = sobaka_pointer->color_getter();
    cout << sobaka_color << endl;
    popugai_pointer->sound();

    
    Bird wtf_bird;
    Bird vorona(4, 1, "black", 10, "yellow");
    Bird sinica = vorona;

    
    Bird* penguin_pointer = new Bird;
    Bird* sokol_pointer = new Bird(10, 1, "white", 100, "black");
    Bird* orel_pointer = new Bird(*sokol_pointer);

    sokol_pointer->sound();


    Animal* kanareika_pointer = new Bird();
    Animal* kanareika2_pointer = new Animal;

    
    kanareika_pointer->sound();
    kanareika2_pointer->sound();


    delete cat_pointer;
    delete sobaka_pointer;
    delete popugai_pointer;
    delete kanareika_pointer;
    delete kanareika2_pointer;

    orel_pointer->sound();
    delete penguin_pointer;
    delete sokol_pointer;
    delete orel_pointer;
    
}


