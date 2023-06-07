import { Injectable } from '@angular/core';
import { finalize, tap } from 'rxjs/operators';
import { Action, Selector, State, StateContext, Store } from '@ngxs/store';
import { patch, updateItem } from '@ngxs/store/operators';

import { ToasterService } from '@abp/ng.theme.shared';
import { PagedResultDto } from '@abp/ng.core';
import {
    GetUnitGroups,
    CreateUnitGroup,
    DeleteUnitGroup,
    GetUnitGroup,
    ClearUnitGroup,
    UpdateUnitGroup,
    UpdateRequestDto,
    SelectUnitGroup,
    ClearSelectedUnitGroup,
    GetUnitGroupByCode,
    GetGlobalUnits
} from '../actions';
import { GlobalUnitDto, UnitDto, UnitGroupDto, UnitGroupService, UnitGroupWithDetailsDto } from '@proxy/units';
import { UNIT_STATE_TOKEN } from '../tokens/unit.tokens';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';

import * as unitConsts from '../consts'

export class UnitStateModel {
    public unitGroups: UnitGroupDto[];
    public requestDto: FilteredPagedAndSortedResultRequestDto;
    public unitGroup: UnitGroupWithDetailsDto;
    public selectedUnitGroup: UnitGroupDto;
    public busy: boolean;
    public totalCount: number;
    public globalUnits: GlobalUnitDto[];
    public error: any;
}

@State<UnitStateModel>({
    name: UNIT_STATE_TOKEN,
    defaults: <UnitStateModel>{
        unitGroups: [],
        requestDto: {
            maxResultCount: unitConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },
        unitGroup: null,
        selectedUnitGroup: null,
        busy: false,
        totalCount: 0,
        error: null
    }
})
@Injectable()
export class UnitState {
    //#region Selectors from state
    @Selector()
    static getUnitGroups(state: UnitStateModel): UnitGroupDto[] {
        return state.unitGroups || [];
    }

    @Selector()
    static getUnitGroup(state: UnitStateModel): UnitGroupWithDetailsDto {
        return state.unitGroup || null;
    }

    @Selector()
    static getUnits(state: UnitStateModel): UnitDto[] {
        return state.unitGroup.units || [];
    }

    @Selector()
    static getRequestDto(state: UnitStateModel): FilteredPagedAndSortedResultRequestDto {
        return state.requestDto;
    }

    @Selector()
    static getGlobalUnits(state: UnitStateModel): GlobalUnitDto[] {
        return state.globalUnits || [];
    }
    //#endregion

    //#region Ctor
    constructor(
        private readonly unitGroupService: UnitGroupService,
        private readonly toasterService: ToasterService,
        private readonly store: Store
    ) { }
    //#endregion

    //#region Actions
    @Action(GetUnitGroups)
    loadUnitGroups(ctx: StateContext<UnitStateModel>, { payload }: GetUnitGroups) {
        return this.unitGroupService.list(payload).pipe(
            tap((response: PagedResultDto<UnitGroupDto>) => {
                const state = ctx.getState();
                ctx.setState({
                    ...state,
                    unitGroups: response.items ?? [],
                    totalCount: response.totalCount
                })
            })
        );
    }

    @Action(UpdateRequestDto)
    updateRequestDto(ctx: StateContext<UnitStateModel>, { payload }: UpdateRequestDto) {
        ctx.setState(patch({ requestDto: payload }));
    }

    @Action(GetUnitGroup)
    loadUnitGroup(ctx: StateContext<UnitStateModel>, { payload }: GetUnitGroup) {
        return this.unitGroupService.get(payload).pipe(
            tap((response: UnitGroupWithDetailsDto) => {
                ctx.setState(patch({ unitGroup: response }));
            })
        );
    }

    @Action(GetUnitGroupByCode)
    loadUnitGroupByCode(ctx: StateContext<UnitStateModel>, { payload }: GetUnitGroupByCode) {
        return this.unitGroupService.getByCode(payload).pipe(
            tap((response: UnitGroupWithDetailsDto) => {
                ctx.setState(patch({ unitGroup: response }));
            })
        );
    }

    @Action(CreateUnitGroup)
    createUnitGroup(ctx: StateContext<UnitStateModel>, { payload }: CreateUnitGroup) {
        ctx.setState(patch({ busy: true }))
        return this.unitGroupService.create(payload).pipe(
            tap((response: UnitGroupDto) => {
                const state = ctx.getState();
                this.store.dispatch(new GetUnitGroups({ ...state.requestDto }));
                this.toasterService.success('::CreatedSuccessfullyMessage', '::SuccessTitleMessage');
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateUnitGroup)
    updateUnitGroup(ctx: StateContext<UnitStateModel>, { id, payload }: UpdateUnitGroup) {
        ctx.setState(patch({ busy: true }))
        return this.unitGroupService.update(id, payload).pipe(
            tap((response: UnitGroupWithDetailsDto) => {
                ctx.setState(patch({
                    unitGroups: updateItem<UnitGroupDto>(f => f.id == response.id, { ...response })
                }))
                this.toasterService.success('::UpdatedSuccessfullyMessage', '::SuccessTitleMessage');
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(DeleteUnitGroup)
    deleteUnitGroup(ctx: StateContext<UnitStateModel>, { payload }: DeleteUnitGroup) {
        return this.unitGroupService.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage')
                const state = ctx.getState();
                ctx.patchState({
                    ...state,
                    unitGroups: state.unitGroups.filter(unitGroup => unitGroup.id !== payload),
                    totalCount: state.totalCount - 1
                });
            })
        );
    }

    @Action(SelectUnitGroup)
    selectUnitGroup(ctx: StateContext<UnitStateModel>, { payload }: SelectUnitGroup) {
        ctx.setState(patch({ selectedUnitGroup: payload }));
    }

    @Action(ClearSelectedUnitGroup)
    clearSelectedUnitGroup(ctx: StateContext<UnitStateModel>) {
        ctx.setState(patch({ selectedUnitGroup: null }))
    }

    @Action(ClearUnitGroup)
    clearUnitGroup(ctx: StateContext<UnitStateModel>) {
        ctx.setState(patch({ unitGroup: null }))
    }

    @Action(GetGlobalUnits)
    loadGlobalUnits(ctx: StateContext<UnitStateModel>) {
        if (ctx.getState().globalUnits === undefined || ctx.getState().globalUnits?.length < 1)
            return this.unitGroupService.getGlobalUnits().pipe(
                tap((response: GlobalUnitDto[]) => {
                    ctx.setState(patch({ globalUnits: response }));
                })
            );
    }
    //#endregion
}
