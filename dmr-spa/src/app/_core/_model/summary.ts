export interface IIngredient {
    id: number;
    scanStatus: boolean;
    code: string;
    scanCode: string;
    materialNO: string;
    name: string;
    percentage: string;
    position: string;
    allow: number;
    expected: any;
    real: number;
    focusReal: boolean;
    focusExpected: boolean;
    valid: boolean;
    info: string;
    batch: string;
    unit: string;
}
