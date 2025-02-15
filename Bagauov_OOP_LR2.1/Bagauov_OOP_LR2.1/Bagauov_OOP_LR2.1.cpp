#include <iostream>
#include <string>

using namespace std;

class CPU {

private:
    string model;
    int core;
public:
    CPU(): core(0), model("") {
        printf("Вызван конструктор по умолчанию класса CPU \n");
    }
    CPU(int core, string model): core(core), model(model) {
        printf("Вызва конструктор с параметрами класса CPU \n");
    }
    CPU(CPU& cpu_copy): core(cpu_copy.core), model(cpu_copy.model) {
        printf("Вызван конструктор копирования класса CPU \n");
    }
    ~CPU() {
        printf("Вызван деструктор класса CPU \n");
    }

    void core_setter(int c) {
        if (core > 0 && core < 512) {
            core = c;
        }
    }

    void model_setter(string m) {
        model = m;
    }

    int core_getter() {
        return core;
    }

    string model_getter() {
        return model;
    }

    void print_info() {
        
        printf("Model info: %d\n", core);
        
        printf("Core info: %s\n", model.c_str());
    }

};





int main() {
    setlocale(LC_ALL, "RU");

    CPU model1;
    CPU model2(6, "Intel");
    CPU model3 = model2;

    CPU* model4 = new CPU();
    CPU* model5 = new CPU(8, "AMD");
    CPU* model6 = new CPU(*model5);

    model2.print_info();

    delete model4;
    delete model5;
    delete model6;

}

