import { PeriodMixing } from "./period.mixing";

export interface PeriodDispatch {
    id: number;
    startTime: Date;
    endTime: Date;
    isDelete: boolean;
    createdBy: number;
    deletedBy: number;
    updatedBy: number;
    createdTime: Date;
    deletedTime: Date | null;
    updatedTime: Date | null;
    periodMixingID: number;
    periodMixing: PeriodMixing;
}
