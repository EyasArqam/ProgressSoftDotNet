type Gender = 'Male' | 'Female'

export class BusinessCard {
    constructor(
        public name: string,
        public email: string,
        public phone: string,
        public address: string,
        public gender: Gender,
        public dateOfBirth?: Date,
        public photo?: string
    ) { }
}
