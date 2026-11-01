export enum Gender {
  Male = 'Male',
  Female = 'Female',
  Other = 'Other',
}

export enum EmployeeStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Resigned = 'Resigned',
  Probation = 'Probation',
}

export enum AttendanceStatus {
  Present = 'Present',
  Absent = 'Absent',
  Late = 'Late',
  EarlyLeave = 'EarlyLeave',
  WorkFromHome = 'WorkFromHome',
  Leave = 'Leave',
}

export enum ContractType {
  Probation = 'Probation',
  DefiniteTerm = 'DefiniteTerm',
  IndefiniteTerm = 'IndefiniteTerm',
  Internship = 'Internship',
  PartTime = 'PartTime',
}

export enum ContractStatus {
  Active = 'Active',
  Expired = 'Expired',
  Terminated = 'Terminated',
}

export enum LeaveType {
  AnnualLeave = 'AnnualLeave',
  SickLeave = 'SickLeave',
  MaternityLeave = 'MaternityLeave',
  SpecialLeave = 'SpecialLeave',
  UnpaidLeave = 'UnpaidLeave',
}

export enum LeaveStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled',
}

export enum BonusPenaltyType {
  Bonus = 1,
  Penalty = 2,
}
