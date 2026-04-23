using LightProto;
using System;
using MemoryPack;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Pool;
using Fantasy.Network.Interface;
using Fantasy.Serialize;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PreferConcreteValueOverDefault
// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable RedundantUsingDirective
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Fantasy
{
    /// <summary>
    /// 客户端请求登录
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2G_LoginRequest : AMessage, IRequest
    {
        public static C2G_LoginRequest Create(bool autoReturn = true)
        {
            var c2G_LoginRequest = MessageObjectPool<C2G_LoginRequest>.Rent();
            c2G_LoginRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_LoginRequest.SetIsPool(false);
            }
            
            return c2G_LoginRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            account = default;
            password = default;
            MessageObjectPool<C2G_LoginRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_LoginRequest; } 
        [ProtoIgnore]
        public G2C_LoginResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string account { get; set; }
        [ProtoMember(2)]
        public string password { get; set; }
    }
    /// <summary>
    /// 服务端返回登录结果
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_LoginResponse : AMessage, IResponse
    {
        public static G2C_LoginResponse Create(bool autoReturn = true)
        {
            var g2C_LoginResponse = MessageObjectPool<G2C_LoginResponse>.Rent();
            g2C_LoginResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_LoginResponse.SetIsPool(false);
            }
            
            return g2C_LoginResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            code = default;
            result = default;
            userId = default;
            MessageObjectPool<G2C_LoginResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_LoginResponse; } 
        [ProtoMember(4)]
        public uint ErrorCode { get; set; }
        [ProtoMember(1)]
        public string code { get; set; }
        [ProtoMember(2)]
        public string result { get; set; }
        [ProtoMember(3)]
        public long userId { get; set; }
    }
    /// <summary>
    /// 客户端登出消息
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2G_SignoutMessage : AMessage, IMessage
    {
        public static C2G_SignoutMessage Create(bool autoReturn = true)
        {
            var c2G_SignoutMessage = MessageObjectPool<C2G_SignoutMessage>.Rent();
            c2G_SignoutMessage.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_SignoutMessage.SetIsPool(false);
            }
            
            return c2G_SignoutMessage;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            userId = default;
            MessageObjectPool<C2G_SignoutMessage>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_SignoutMessage; } 
        [ProtoMember(1)]
        public long userId { get; set; }
    }
}