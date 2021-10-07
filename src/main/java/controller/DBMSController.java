package controller;
import domain.Database;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;
import service.DBMSService;

@RestController
@RequestMapping(path="api")
public class DBMSController {

    @Autowired
    private DBMSService service;

    @PostMapping(value="database")
    public Database createDatabase(@RequestParam("name") String name){
        return this.service.createDatabase(name);
    }
}
