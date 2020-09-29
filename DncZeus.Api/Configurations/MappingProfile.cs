/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.ViewModels.Finance.Account;
using DncZeus.Api.ViewModels.Finance.FinanceInfo;
using DncZeus.Api.ViewModels.Rbac.DncIcon;
using DncZeus.Api.ViewModels.Rbac.DncMenu;
using DncZeus.Api.ViewModels.Rbac.DncPermission;
using DncZeus.Api.ViewModels.Rbac.DncRole;
using DncZeus.Api.ViewModels.Rbac.DncUser;
using DncZeus.Api.ViewModels.Refuse;
using DncZeus.Api.ViewModels.System.Dictionary;
using DncZeus.Api.ViewModels.System.DicType;
using DncZeus.Api.ViewModels.User.Department;
using DncZeus.Api.ViewModels.User.Position;
using DncZeus.Api.ViewModels.Wage;

namespace DncZeus.Api.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public MappingProfile()
        {
            #region DncUser
            CreateMap<DncUser, UserJsonModel>();
            CreateMap<UserCreateViewModel, DncUser>();
            CreateMap<UserEditViewModel, DncUser>();
            CreateMap<DncUser, UserEditViewModel>();
            #endregion

            #region DncRole
            CreateMap<DncRole, RoleJsonModel>();
            CreateMap<RoleCreateViewModel, DncRole>();
            CreateMap<DncRole, RoleCreateViewModel>();
            #endregion

            #region DncMenu
            CreateMap<DncMenu, MenuJsonModel>();
            CreateMap<MenuCreateViewModel, DncMenu>();
            CreateMap<MenuEditViewModel, DncMenu>();
            CreateMap<DncMenu, MenuEditViewModel>();
            #endregion

            #region DncIcon
            CreateMap<DncIcon, IconCreateViewModel>();
            CreateMap<DncIcon, IconJsonModel>();
            CreateMap<IconCreateViewModel, DncIcon>();
            #endregion

            #region DncPermission
            CreateMap<DncPermission, PermissionJsonModel>()
                .ForMember(d => d.MenuName, s => s.MapFrom(x => x.Menu.Name))
                .ForMember(d => d.PermissionTypeText, s => s.MapFrom(x => x.Type.ToString()));
            CreateMap<PermissionCreateViewModel, DncPermission>();
            CreateMap<PermissionEditViewModel, DncPermission>();
            CreateMap<DncPermission, PermissionEditViewModel>();
            #endregion

            #region UerDepartment
            CreateMap<UserDepartment, DepartmentJsonModel>();
            CreateMap<UserDepartment, DepartmentCreateViewModel>();
            CreateMap<DepartmentCreateViewModel, UserDepartment>().
                ForMember(d => d.Monday, s => s.MapFrom(x => x.WorkTime)).
                ForMember(d => d.Thursday, s => s.MapFrom(x => x.WorkTime)).
                ForMember(d => d.Tuesday, s => s.MapFrom(x => x.WorkTime)).
                ForMember(d => d.Wednesday, s => s.MapFrom(x => x.WorkTime)).
                ForMember(d => d.Friday, s => s.MapFrom(x => x.WorkTime)).
                ForMember(d => d.Saturday, s => s.MapFrom(x => x.WorkTime)).
                ForMember(d => d.Sunday, s => s.MapFrom(x => x.WorkTime));
            #endregion

            #region UerPosition
            CreateMap<UserPosition, PositionJsonModel>();
            CreateMap<UserPosition, PositionCreateViewModel>();
            CreateMap<PositionCreateViewModel, UserPosition>();
            #endregion

            #region ResumeInfo
            CreateMap<ResumeInfo, ResumeJsonModel>();
            CreateMap<ResumeInfo, ResumeCreateViewModel>();
            CreateMap<ResumeCreateViewModel, ResumeInfo>();
            #endregion

            #region WageInfo
            CreateMap<WageInfo, WageJsonModel>();
            CreateMap<WageInfo, WageCreateViewModel>();
            CreateMap<WageCreateViewModel, WageInfo>();
            #endregion

            #region FinanceAccount
            CreateMap<FinanceAccount, AccountJsonModel>();
            CreateMap<FinanceAccount, AccountCreateViewModel>();
            CreateMap<AccountCreateViewModel, FinanceAccount>();
            #endregion

            #region FinanceInfo
            CreateMap<FinanceInfo, FinanceInfoJsonModel>();
            CreateMap<FinanceInfo, FinanceInfoCreateViewModel>();
            CreateMap<FinanceInfoCreateViewModel, FinanceInfo>();
            #endregion

            #region SystemDictionary
            CreateMap<SystemDictionary, DictionaryJsonModel>();
            CreateMap<SystemDictionary, DictionaryCreateViewModel>();
            CreateMap<DictionaryCreateViewModel, SystemDictionary>();
            #endregion

            #region SystemDicType
            CreateMap<SystemDicType, DicTypeJsonModel>();
            CreateMap<SystemDicType, DicTypeCreateViewModel>();
            CreateMap<DicTypeCreateViewModel, SystemDicType>();
            #endregion
        }
    }
}
