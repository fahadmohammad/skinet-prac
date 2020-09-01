let data: number | string;
data = 42;
data = '42';

interface ICar{

    color: string;
    model: string;
    topSpeed?: number;
}

const car1: ICar = {
    color: 'blue',
    model: 'bmw'
};
const car2: ICar = {
    color: 'blue',
    model: 'bmw',
    topSpeed: 200
};

const multiply = (x: number,y: number) => {
    return x * y;
};
