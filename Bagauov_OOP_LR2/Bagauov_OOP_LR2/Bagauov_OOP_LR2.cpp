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
        printf("Вызван базовый конструктор класса Animal \n");
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

    void sound();
};

void Animal::sound() {
    printf("*silence*...\n");
}

int main() {
    setlocale(LC_ALL, "RU");
    Animal wtf_animal;
    Animal cat(1, 3, "black");
    Animal cat2 = cat;


    Animal* cat_pointer = new Animal;
    Animal* sobaka_pointer = new Animal(1, 4, "green");
    Animal* popugai_pointer = new Animal(cat);

    cat.sound();
    string sobaka_color = sobaka_pointer->color_getter();
    cout << sobaka_color << endl;
    
    popugai_pointer->sound();

    delete cat_pointer;
    delete sobaka_pointer;
    delete popugai_pointer;
    

    
}


