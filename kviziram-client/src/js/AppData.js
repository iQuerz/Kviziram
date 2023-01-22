
class AppData {
    static Account = undefined;
    static CurrentMatch = undefined;
    
    static loadTestData(){
        AppData.Account = {
            username: "iQuerz",
        }
        AppData.CurrentMatch = {
            inviteCode: "123456",//idk piksi stavi ovde nes"
        }
    }
    static removeTestData(){
        AppData.Account = undefined;
        AppData.CurrentMatch = undefined;
    }
}

export default AppData