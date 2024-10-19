export class BusinessCard {
    constructor(
        public name: string,
        public gender: number,
        public dateOfBirth: Date,
        public email: string,
        public phone: string,
        public address: string,
        public photo?: string
    ) { }
}
